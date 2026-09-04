import { useEffect, useMemo, useState } from "react"
import { Carousel } from "react-bootstrap"
import { FiChevronLeft, FiChevronRight, FiMaximize2, FiX } from "react-icons/fi"

const DEFAULT_PLACEHOLDER = "/noPhoto.png"

export default function ImageGallery({ images = [], alt = "Фото", className = "" }) {
    const safeImages = useMemo(() => {
        const unique = [...new Set(images.filter(Boolean))]
        return unique.length ? unique : [DEFAULT_PLACEHOLDER]
    }, [images])

    const [active, setActive] = useState(0)
    const [fullscreen, setFullscreen] = useState(false)

    useEffect(() => {
        if (active >= safeImages.length) setActive(0)
    }, [active, safeImages.length])

    useEffect(() => {
        if (!fullscreen) return undefined
        const onKeyDown = (event) => {
            if (event.key === "Escape") setFullscreen(false)
            if (event.key === "ArrowLeft") setActive((value) => (value - 1 + safeImages.length) % safeImages.length)
            if (event.key === "ArrowRight") setActive((value) => (value + 1) % safeImages.length)
        }
        const previousOverflow = document.body.style.overflow
        document.body.style.overflow = "hidden"
        window.addEventListener("keydown", onKeyDown)
        return () => {
            document.body.style.overflow = previousOverflow
            window.removeEventListener("keydown", onKeyDown)
        }
    }, [fullscreen, safeImages.length])

    return (
        <>
            <div className={`image-gallery ${className}`}>
                <div className="image-gallery__main">
                    <Carousel
                        activeIndex={active}
                        onSelect={setActive}
                        controls={safeImages.length > 1}
                        indicators={false}
                        interval={null}
                        touch
                    >
                        {safeImages.map((src, index) => (
                            <Carousel.Item key={`${src}-${index}`}>
                                <button
                                    type="button"
                                    className="image-gallery__zoom-trigger"
                                    onClick={() => setFullscreen(true)}
                                    aria-label="Открыть фото на весь экран"
                                >
                                    <img src={src} alt={`${alt} ${index + 1}`} className="image-gallery__main-image" />
                                    {src !== DEFAULT_PLACEHOLDER && (
                                        <span className="image-gallery__zoom-hint">
                                            <FiMaximize2 /> Нажмите, чтобы увеличить
                                        </span>
                                    )}
                                </button>
                            </Carousel.Item>
                        ))}
                    </Carousel>
                </div>

                {safeImages.length > 1 && (
                    <div className="image-gallery__thumbs" role="tablist" aria-label="Миниатюры фотографий">
                        {safeImages.map((src, index) => (
                            <button
                                type="button"
                                key={`${src}-thumb-${index}`}
                                className={`image-gallery__thumb ${index === active ? "is-active" : ""}`}
                                onClick={() => setActive(index)}
                                aria-label={`Фото ${index + 1}`}
                            >
                                <img src={src} alt="" />
                            </button>
                        ))}
                    </div>
                )}
            </div>

            {fullscreen && (
                <div className="gallery-lightbox" role="dialog" aria-modal="true" aria-label="Просмотр фотографии">
                    <button type="button" className="gallery-lightbox__close" onClick={() => setFullscreen(false)} aria-label="Закрыть">
                        <FiX />
                    </button>
                    {safeImages.length > 1 && (
                        <>
                            <button
                                type="button"
                                className="gallery-lightbox__nav gallery-lightbox__nav--left"
                                onClick={() => setActive((value) => (value - 1 + safeImages.length) % safeImages.length)}
                                aria-label="Предыдущее фото"
                            >
                                <FiChevronLeft />
                            </button>
                            <button
                                type="button"
                                className="gallery-lightbox__nav gallery-lightbox__nav--right"
                                onClick={() => setActive((value) => (value + 1) % safeImages.length)}
                                aria-label="Следующее фото"
                            >
                                <FiChevronRight />
                            </button>
                        </>
                    )}
                    <img className="gallery-lightbox__image" src={safeImages[active]} alt={`${alt} ${active + 1}`} />
                    <div className="gallery-lightbox__counter">{active + 1} / {safeImages.length}</div>
                </div>
            )}
        </>
    )
}
