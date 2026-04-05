---
description: Validate PDF certificate rendering logic
---

# PDF Generation Validation

Use this checklist when modifying `ContractManager.cs` or `FileStoringManager.cs` related to PDF certificates.

1. **Color Validation**: Ensure NO named colors (like "black", "red") are passed to `iTextSharp`. Always use Hex strings:
    - **Correct**: `#000000`
    - **Incorrect**: `black`

2. **Font Integrity**: Verify that `times.ttf` is correctly referenced from `wwwroot/font/`.

3. **Try-Catch Coverage**: Ensure all new PDF rendering logic is wrapped in a `try-catch` that throws a `UserFriendlyException` with a descriptive message.

4. **Status Localization**: Check that `ContractStatus` switch cases are updated in the `RenderCertificatePdf` methods:
```csharp
case ContractStatus.Draft: statusName = "Draft"; break;
```
