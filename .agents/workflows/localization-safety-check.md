---
description: Scan for hardcoded Vietnamese strings
---

# Localization Safety Check

Run this workflow before every commit to ensure no Vietnamese strings are hardcoded in the codebase.

1. Run the global grep scan:
```bash
grep -rnE "á|à|ả|ã|ạ|ă|ắ|ằ|ẳ|ẵ|ặ|â|ấ|ầ|ẩ|ẫ|ậ|é|è|ẻ|ẽ|ẹ|ê|ế|ề|ể|ễ|ệ|í|ì|ỉ|ĩ|ị|ó|ò|ỏ|õ|ọ|ô|ố|ồ|ổ|ỗ|ộ|ơ|ớ|ờ|ở|ỡ|ợ|ú|ù|ủ|ũ|ụ|ư|ứ|ừ|ử|ữ|ự|ý|ỳ|ỷ|ỹ|ỵ|đ" . --exclude-dir={.git,node_modules,bin,obj,dist,.gemini} --exclude="*.xml"
```

2. If any results appear outside of comments or localization files:
    - Identify the corresponding key in `eContract-en.xml`.
    - Replace the hardcoded string with the localization call (e.g., `L("Key")` in backend or `{{ "Key" | ecTranslate }}` in frontend).

> [!TIP]
> Always prefer adding new strings to the XML sources rather than hardcoding them to maintain multi-language support.
