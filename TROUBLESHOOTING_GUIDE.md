# Troubleshooting Guide

Common issues and resolutions for the Metasign platform.

## PDF Generation Errors

### "Status 500" during Contract Creation
- **Symptom**: Creating a contract from a template or file fails with a generic "Internal Error" or Status 500.
- **Root Cause**: Often a hex color parsing failure or storage connection issue.
- **Resolution**:
    - Check for **named colors** (e.g., "black") in `ContractManager.cs`. These must be changed to hex codes like `#000000`.
    - Use the **`pdf-generation-validation`** workflow to verify changes.

## File Storage Issues

### "Failed to upload contract"
- **Symptom**: Files are selected but do not save to the draft.
- **Root Cause**: GCS or AWS S3 bucket permissions or connectivity.
- **Resolution**:
    - Verify `FileStoring:Provider` in `appsettings.json`.
    - Ensure your service account has `Storage Object Admin` permissions on the bucket.

## Authentication Failures

### Google Auth Login Error
- **Symptom**: "Login Failed" when using Google.
- **Resolution**:
    - Ensure your Redirect URI is correctly whitelisted in the Google Cloud Console (`/api/TokenAuth/ExternalLoginCallback`).
    - Verify `Authentication:Google:ClientId` in `appsettings.json`.
