import { useEffect, useState } from "react";

const DEFAULT_PLACEHOLDER = "/noPhoto.png";

export function useImage(url) {
    const [src, setSrc] = useState(DEFAULT_PLACEHOLDER);

    useEffect(() => {
        if (!url) return;

        const img = new Image();

        img.onload = () => setSrc(url);
        img.onerror = () => setSrc(DEFAULT_PLACEHOLDER);

        img.src = url;
    }, [url]);

    return src;
}