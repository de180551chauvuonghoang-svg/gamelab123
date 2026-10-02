import math
import struct
import wave
import os

def create_wav(filename, sample_rate, duration, generator_func):
    num_samples = int(sample_rate * duration)
    with wave.open(filename, 'w') as wav_file:
        wav_file.setnchannels(1)      # Mono
        wav_file.setsampwidth(2)      # 16-bit
        wav_file.setframerate(sample_rate)
        
        frames = bytearray()
        for i in range(num_samples):
            t = i / sample_rate
            val = generator_func(t, duration)
            val = max(-1.0, min(1.0, val))
            sample = int(val * 32767.0)
            frames.extend(struct.pack('<h', sample))
            
        wav_file.writeframes(frames)
    print(f"Created {filename} ({duration:.2f}s)")

# 1. BGM (Catchy 8-bit Chiptune loop - 4 bars, ~4 seconds)
def bgm_synth(t, duration):
    # Tempo: 128 BPM -> 1 beat = 60/128 = 0.46875s
    # Chords: C (261.6), G (196.0), Am (220.0), F (174.6)
    beat_len = 0.46875
    measure = int(t / (beat_len * 4)) % 4
    beat = int(t / beat_len) % 4
    subbeat = int((t % beat_len) / (beat_len / 4)) % 4
    
    # Root frequencies for chords
    chord_roots = [261.63, 196.00, 220.00, 174.61] # C4, G3, A3, F3
    root = chord_roots[measure]
    
    # Bass line (Square wave)
    bass_freq = root / 2.0
    bass = 1.0 if math.sin(2 * math.pi * bass_freq * t) > 0 else -1.0
    bass_env = math.exp(-6.0 * (t % (beat_len * 2)))
    bass *= bass_env * 0.35
    
    # Arpeggio melody (Pulse wave with slight vibrato)
    notes_c = [261.63, 329.63, 392.00, 523.25] # C, E, G, C5
    notes_g = [196.00, 246.94, 293.66, 392.00] # G, B, D, G4
    notes_am = [220.00, 261.63, 329.63, 440.00] # A, C, E, A4
    notes_f = [174.61, 220.00, 261.63, 349.23] # F, A, C, F4
    chord_notes = [notes_c, notes_g, notes_am, notes_f][measure]
    
    # Melody step in 16th notes
    step = (beat * 4 + subbeat) % 16
    arp_pattern = [0, 1, 2, 3, 2, 1, 3, 2, 0, 2, 1, 3, 2, 3, 1, 0]
    note_idx = arp_pattern[step]
    freq = chord_notes[note_idx]
    
    note_t = t % (beat_len / 4)
    # Triangle / pulse mix for melody
    sine = math.sin(2 * math.pi * freq * t)
    pulse = 1.0 if (t * freq) % 1.0 < 0.35 else -1.0
    mel_env = math.exp(-12.0 * note_t)
    melody = (sine * 0.4 + pulse * 0.25) * mel_env * 0.35
    
    # Light hi-hat tick on 8th notes
    hihat_t = t % (beat_len / 2)
    noise = (math.sin(t * 12345.67) * 43758.5453) % 1.0 - 0.5
    hihat = noise * math.exp(-40.0 * hihat_t) * 0.15
    
    return bass + melody + hihat

# 2. Jump Sound (Retro pitch glide up)
def jump_synth(t, duration):
    progress = t / duration
    freq = 160.0 + 520.0 * (progress ** 1.4)
    phase = 2 * math.pi * freq * t
    wave_val = 1.0 if (t * freq) % 1.0 < 0.5 else -1.0
    env = 1.0 - progress
    return wave_val * env * 0.45

# 3. Coin Chime (Two-tone crisp bell ding: B5 -> E6)
def coin_synth(t, duration):
    t_split = 0.08
    if t < t_split:
        f = 987.77 # B5
        env = math.exp(-8.0 * t)
    else:
        f = 1318.51 # E6
        env = math.exp(-9.0 * (t - t_split))
    sine = math.sin(2 * math.pi * f * t)
    harmonic = math.sin(2 * math.pi * f * 2 * t) * 0.25
    return (sine + harmonic) * env * 0.5

# 4. Game Over (Descending sad slide)
def gameover_synth(t, duration):
    progress = t / duration
    freq = 420.0 * math.exp(-2.2 * progress)
    wave_val = 1.0 if math.sin(2 * math.pi * freq * t) > 0 else -1.0
    env = (1.0 - progress) * 0.4
    return wave_val * env

# 5. Victory Win Fanfare (Rapid celebratory arpeggio: C5 -> E5 -> G5 -> C6)
def win_synth(t, duration):
    notes = [523.25, 659.25, 783.99, 1046.50]
    num_notes = len(notes)
    step_duration = duration / num_notes
    step = min(int(t / step_duration), num_notes - 1)
    f = notes[step]
    note_t = t - step * step_duration
    env = math.exp(-4.0 * note_t) if step < num_notes - 1 else math.exp(-1.5 * note_t)
    wave_val = math.sin(2 * math.pi * f * t) + 0.3 * math.sin(4 * math.pi * f * t)
    return wave_val * env * 0.45

# 6. Button Click (Crisp pop click)
def click_synth(t, duration):
    freq = 1200.0 * math.exp(-25.0 * t)
    env = math.exp(-50.0 * t)
    return math.sin(2 * math.pi * freq * t) * env * 0.5

if __name__ == '__main__':
    target_dir = os.path.join(os.path.dirname(__file__), '..', 'My project (5)', 'Assets', 'Audio')
    os.makedirs(target_dir, exist_ok=True)
    
    sr = 44100
    create_wav(os.path.join(target_dir, 'bgm.wav'), sr, 7.5, bgm_synth)
    create_wav(os.path.join(target_dir, 'jump.wav'), sr, 0.22, jump_synth)
    create_wav(os.path.join(target_dir, 'coin.wav'), sr, 0.38, coin_synth)
    create_wav(os.path.join(target_dir, 'gameover.wav'), sr, 1.2, gameover_synth)
    create_wav(os.path.join(target_dir, 'win.wav'), sr, 1.4, win_synth)
    create_wav(os.path.join(target_dir, 'click.wav'), sr, 0.08, click_synth)
    print("All audio files generated successfully!")
