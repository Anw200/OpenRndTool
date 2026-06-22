OpenRndTool — Deterministic Secret‑Bound Random Stream Generator
OpenRndTool is a deterministic random‑stream generator that produces reproducible, cryptographically strong byte streams based on a user‑supplied secret.
The generator uses SHA‑512 as its core primitive and binds every output block to the secret, ensuring that no part of the stream can be reproduced or extended without knowing the secret.

How the Generator Works
1. Initial Seed Derived From the Secret.
The process begins by hashing the user’s secret:

Code
state₀ = SHA512(secret)
This 64‑byte hash becomes the initial state for the generator.
It ensures that:

the same secret always produces the same stream

different secrets produce completely unrelated streams

no information about the secret is leaked

2. Each Following Block Depends on BOTH the Previous Block and the Secret
After the initial block, every subsequent block is computed as:

Code
stateₙ₊₁ = SHA512(stateₙ || secret)
Where:

stateₙ is the previous 64‑byte block

secret is the original user‑supplied secret

|| means concatenation

This creates a keyed hash chain, meaning:

Every block depends on the secret

Every block depends on all previous blocks

No attacker can continue the stream without the secret

No attacker can reverse the stream to recover earlier blocks

This is fundamentally different from a normal SHA‑512 chain, where knowing any block allows you to compute all future blocks.
Here, the secret is required at every step.

Why This Design Is Secure
✔ Secret‑Bound at Every Step
Even if someone obtains a 64‑byte block from the output, they cannot compute the next block without the secret.

✔ Deterministic
Same secret → same output stream.
Different secret → completely different stream.

✔ Cryptographically Strong
SHA‑512 provides:

512‑bit output

strong avalanche behavior

resistance to collisions and preimage attacks

✔ No Reseeding Weaknesses
The secret is mixed into every block, not just the first one.

Output Format
The generator writes the output stream to a text file as:

uppercase hexadecimal

fixed number of bytes per row (user‑configurable)

optional line breaks (0 = continuous stream)

Example:

Code
A3F1C0... (256 bytes per row)
Standard Random Mode
The tool also includes a standard .NET Random mode, seeded using:

Code
seed = first 32 bits of SHA512(secret)
This mode is not cryptographically secure, but useful for testing and comparison.

Use Cases
deterministic random file generation

reproducible test data

secret‑based key material expansion

offline random stream generation

cryptographic research and experimentation

License
Free for personal and research use.
No warranty. Use at your own risk.
