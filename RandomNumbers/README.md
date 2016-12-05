Random Number Generators
================

| Algorithm                          | SD mean (σ) | Mean iterations | Exp value | Min value | Max value |
| ---------------------------------- | ----------- | --------------- | --------- | --------- | --------- |
| Math.Random (NodeJS)               | 32.5830     | 100 | 1000 | 842 | 1156 |
| Crypto.GetRandomBytes (NodeJS)     | 31.6370     | 100 | 1000 | 844 | 1162|
| Math.Random (PlayFab JS Engine)    | 32.2664     | 3   | 1000 | | |
| Math.Random (Chrome V8)            | 32.5733     | 100 | 1000 | 835 | 1162 |
| Crypto.GetRandomValues (Chrome V8) | 31.6355     | 100 | 1000 | 851 | 1177 |

- Index array length = 16384
- Iterations per call = 16384 * 1000
- Expected value per index = 1000
