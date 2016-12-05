Random Number Generators
================

| Algorithm                              | SD mean (σ) | Mean iterations | Min value | Max value |
| -------------------------------------- | ----------- | --------------- | --------- | --------- |
| Math.Random (NodeJS)                   | 32.5830     | 100 | 842 | 1156 |
| Crypto.GetRandomBytes (NodeJS)         | 31.6370     | 100 | 844 | 1162|
| Math.Random (PlayFab JS Engine)        | 32.2664     | 3   | 871* | 1136* |
| Math.Random (Chrome V8)                | 32.5733     | 100 | 835 | 1162 |
| Crypto.GetRandomValues (Chrome V8)     | 31.6355     | 100 | 851 | 1177 |
| Random.NextBytes (Mono C#, clock seed) | 31.6648     | 100 | 850 | 1165 |
| CrypoServiceProvider (Mono C#)         | 31.6096     | 100 | 852 | 1160 |
| Random.Range (Unity, no seed)          | 32.1463     | 100 | 437 | 1169 |
| Random.Range (Unity, CSP seed)         | 32.1351     | 100 | 431 | 1169 |
| WELL512 (CSP seed)                     |      | 100 | | |


- Index array length = 16384
- Iterations per call = 16384 * 1000 = 16.384.000
- Expected value per index = 1000
