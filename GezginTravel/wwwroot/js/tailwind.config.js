window.tailwind = window.tailwind || {};

tailwind.config = {
    darkMode: "class",
    theme: {
        extend: {
            "colors": {
                "surface-container-high": "#eae7e7",
                "on-secondary-container": "#004b84",
                "primary-fixed": "#ffdbd0",
                "surface-container-highest": "#e5e2e1",
                "primary-container": "#ff6b35",
                "on-tertiary-fixed-variant": "#52443d",
                "surface": "#fcf9f8",
                "error-container": "#ffdad6",
                "on-tertiary-container": "#3c2f28",
                "on-primary-fixed": "#390c00",
                "on-tertiary": "#ffffff",
                "tertiary-fixed": "#f4ded4",
                "surface-container-low": "#f6f3f2",
                "on-background": "#1c1b1b",
                "primary-fixed-dim": "#ffb59d",
                "tertiary": "#6b5b53",
                "on-surface-variant": "#594139",
                "on-secondary": "#ffffff",
                "tertiary-container": "#a9968d",
                "primary": "#ab3500",
                "inverse-primary": "#ffb59d",
                "on-error-container": "#93000a",
                "surface-tint": "#ab3500",
                "outline-variant": "#e1bfb5",
                "surface-container-lowest": "#ffffff",
                "inverse-on-surface": "#f3f0ef",
                "surface-bright": "#fcf9f8",
                "on-primary-container": "#5f1900",
                "on-surface": "#1c1b1b",
                "on-error": "#ffffff",
                "surface-container": "#f0eded",
                "on-secondary-fixed": "#001c37",
                "on-secondary-fixed-variant": "#004880",
                "outline": "#8d7168",
                "surface-variant": "#e5e2e1",
                "secondary-container": "#87bcfe",
                "background": "#fcf9f8",
                "secondary-fixed": "#d2e4ff",
                "secondary": "#24619d",
                "inverse-surface": "#313030",
                "on-primary": "#ffffff",
                "secondary-fixed-dim": "#a1c9ff",
                "surface-dim": "#dcd9d9",
                "on-primary-fixed-variant": "#832600",
                "tertiary-fixed-dim": "#d7c2b9",
                "error": "#ba1a1a",
                "on-tertiary-fixed": "#241913"
            },
            "borderRadius": {
                "DEFAULT": "0.25rem",
                "lg": "0.5rem",
                "xl": "0.75rem",
                "full": "9999px"
            },
            "spacing": {
                "margin-desktop": "40px",
                "base": "8px",
                "margin-mobile": "16px",
                "container-max": "1280px",
                "gutter": "24px"
            },
            "fontFamily": {
                "body-md": ["Inter"],
                "display-lg-mobile": ["Montserrat"],
                "label-caps": ["Inter"],
                "body-lg": ["Inter"],
                "headline-md": ["Montserrat"],
                "display-lg": ["Montserrat"]
            },
            "fontSize": {
                "body-md": ["16px", { "lineHeight": "24px", "fontWeight": "400" }],
                "display-lg-mobile": ["32px", { "lineHeight": "40px", "letterSpacing": "-0.01em", "fontWeight": "700" }],
                "label-caps": ["12px", { "lineHeight": "16px", "letterSpacing": "0.05em", "fontWeight": "700" }],
                "body-lg": ["18px", { "lineHeight": "28px", "fontWeight": "400" }],
                "headline-md": ["24px", { "lineHeight": "32px", "fontWeight": "600" }],
                "display-lg": ["48px", { "lineHeight": "56px", "letterSpacing": "-0.02em", "fontWeight": "700" }]
            }
        }
    }
}