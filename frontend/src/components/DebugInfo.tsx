import { useWindowSize } from "@/lib/hooks/useWindowSize"

const DebugInfo = () => {
  const windowSize = useWindowSize()

  if (process.env.NODE_ENV !== "development") {
    return null
  }

  return (
    <div className="flex flex-row items-center justify-end gap-2 px-5">
      <div className="rounded border px-2 text-xs sm:hidden">xs</div>
      <div className="hidden rounded border px-2 text-xs sm:block md:hidden">
        sm
      </div>
      <div className="hidden rounded border px-2 text-xs md:block lg:hidden">
        md
      </div>
      <div className="hidden rounded border px-2 text-xs lg:block xl:hidden">
        lg
      </div>
      <div className="hidden rounded border px-2 text-xs xl:block 2xl:hidden">
        xl
      </div>
      <div className="3xl:hidden hidden rounded border px-2 text-xs 2xl:block">
        2xl
      </div>
      <div className="3xl:block 4xl:hidden hidden rounded border px-2 text-xs">
        3xl
      </div>
      <div className="4xl:block 5xl:hidden hidden rounded border px-2 text-xs">
        4xl
      </div>
      <div className="text-xs"> | </div>
      <div className="vsm:hidden rounded border px-2 text-xs">vxs</div>
      <div className="vsm:block vlg:hidden hidden rounded border px-2 text-xs">
        vsm
      </div>
      <div className="vlg:block vxl:hidden hidden rounded border px-2 text-xs">
        vlg
      </div>
      <div className="vxl:block hidden rounded border px-2 text-xs">vxl</div>
      <div className="text-muted-foreground text-xs">
        {windowSize.width}x{windowSize.height}
      </div>
      <div className="text-muted-foreground text-xs">MobiFlight 2025</div>
      <div className="text-muted-foreground text-xs">Version 1.0.0</div>
    </div>
  )
}

export default DebugInfo
