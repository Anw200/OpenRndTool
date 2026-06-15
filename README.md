# OpenRndTool  
Deterministic SHA‑512–based random file generator with mixed‑seed logic, preview block output, and SHA‑256 verification.

## Overview
OpenRndTool is a lightweight Windows utility that generates **deterministic random files** using a custom SHA‑512–driven PRNG pipeline.  
Given the same input secret, the tool will always produce the **exact same output file**, making it ideal for:

- reproducible randomness  
- shared‑secret workflows  
- deterministic key‑expansion  
- offline cryptographic experiments  
- generating large entropy pools for downstream tools  

This project is intentionally simple, transparent, and free to use.

---

## Features

### 🔐 Deterministic PRNG
- Uses SHA‑512 as the core mixing and expansion function  
- Combines user input with internal seed‑mixing logic  
- Produces identical output for identical inputs  

### 📄 Random File Generation
- Generates files of user‑selected size (in kilobytes)  
- Writes output as hex‑encoded random data  
- Includes a preview block showing the first 256 bytes  

### 🧪 SHA‑256 File Hashing
- Automatically computes the SHA‑256 fingerprint of the generated file  
- Allows easy verification and reproducibility  

### 🔁 Seed Mixing
- User secret is mixed with internal state  
- Ensures high entropy and deterministic evolution  

---

## How It Works
1. User enters a secret phrase.  
2. The tool mixes the secret using SHA‑512.  
3. A deterministic PRNG loop expands the seed into a large byte stream.  
4. The output is written to a file with a timestamp‑based name.  
5. A preview block is shown in the UI.  
6. The final file’s SHA‑256 hash is computed and displayed.

Because the process is deterministic, anyone with the same secret and the same tool can regenerate the exact same file.

---

## Use Cases
- Creating reproducible entropy for other cryptographic tools  
- Deriving long secrets
