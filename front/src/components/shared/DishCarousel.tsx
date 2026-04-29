import { useCallback, useEffect, useState } from 'react'
import useEmblaCarousel from 'embla-carousel-react'
import { imageUrl } from '@/utils/image-url'

interface DishCarouselProps {
  photos: string[]
  alt: string
}

export default function DishCarousel({ photos, alt }: DishCarouselProps) {
  if (photos.length === 0) {
    return null
  }

  if (photos.length === 1) {
    return (
      <img
        className="pm-modal-photo"
        src={imageUrl(photos[0])!}
        alt={alt}
      />
    )
  }

  return <Carousel photos={photos} alt={alt} />
}

function Carousel({ photos, alt }: DishCarouselProps) {
  const [emblaRef, emblaApi] = useEmblaCarousel({ loop: false, align: 'start' })
  const [selectedIndex, setSelectedIndex] = useState(0)

  const onSelect = useCallback(() => {
    if (!emblaApi) return
    setSelectedIndex(emblaApi.selectedScrollSnap())
  }, [emblaApi])

  useEffect(() => {
    if (!emblaApi) return
    onSelect()
    emblaApi.on('select', onSelect)
    return () => {
      emblaApi.off('select', onSelect)
    }
  }, [emblaApi, onSelect])

  return (
    <div className="pm-carousel">
      <div className="pm-carousel-viewport" ref={emblaRef}>
        <div className="pm-carousel-track">
          {photos.map((path, i) => (
            <div className="pm-carousel-slide" key={`${path}-${i}`}>
              <img
                className="pm-modal-photo"
                src={imageUrl(path)!}
                alt={alt}
                draggable={false}
              />
            </div>
          ))}
        </div>
      </div>

      <div className="pm-carousel-counter">
        {selectedIndex + 1} / {photos.length}
      </div>

      <div className="pm-carousel-dots">
        {photos.map((_, i) => (
          <button
            key={i}
            className={`pm-carousel-dot${i === selectedIndex ? ' is-active' : ''}`}
            onClick={() => emblaApi?.scrollTo(i)}
            aria-label={`Перейти к фото ${i + 1}`}
            type="button"
          />
        ))}
      </div>
    </div>
  )
}
