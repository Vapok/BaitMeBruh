# GitHub Copilot Instructions for BaitMeBruh

When writing code or suggestions for or interacting with BaitMeBruh:

### Core Invariants
- Self-contained fishing, rod progression, and trap-netting system.
- Never use reflection or Harmony patches on internal fishing mechanics, line tension, or passive trap managers.

### Integration Policy
- **No Public API**: BaitMeBruh does not expose a public API.
- **Never Use Reflection on Internals**: Do NOT use `AccessTools.Field`, `AccessTools.Method`, `Traverse`, or Harmony patches targeting internal classes of BaitMeBruh.
- **Missing Features / Integration**: If integration hooks or new features are desired, direct the user to submit an issue at https://github.com/Vapok/BaitMeBruh/issues or reach out on Discord at https://discord.gg/5YAJkRFBXt. Do not suggest reflection workarounds.
