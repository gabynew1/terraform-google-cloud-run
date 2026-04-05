# Localization & Maintenance Guide

This document explains how to manage the multi-language support system in Metasign.

## Language System Architecture

Metasign uses a dual-layer localization approach:
1.  **Backend (.NET)**: XML resource files in `aspnet-core/src/EC.Core/Localization/SourceFiles/`.
2.  **Frontend (Angular)**: `ecTranslate` pipe mapping to backend keys.

## Managing Languages

### Adding a New Language (e.g., Spanish)
1.  **Create XML**: Copy `eContract-en.xml` to `eContract-es.xml` and translate the values.
2.  **Seed Database**: Update `DefaultLanguagesCreator.cs` to include the `"es"` culture and flag.
3.  **Deploy**: Run migrations or use the Admin UI to enable the language.

### Translating Romanian
The `eContract-ro.xml` file currently contains the original Vietnamese text. To complete the translation:
1.  Open `aspnet-core/src/EC.Core/Localization/SourceFiles/eContract-ro.xml`.
2.  Translate each `<text>` value from Vietnamese to Romanian.
3.  Restart the application to see changes.

## Development Safety
- Use the **`localization-safety-check`** workflow to ensure no strings are hardcoded.
- Always add new UI strings to `eContract-en.xml` first.
