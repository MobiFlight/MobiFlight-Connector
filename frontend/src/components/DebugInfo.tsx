import { useWindowSize } from "@/lib/hooks/useWindowSize"

const DebugInfo = () => {
  const windowSize = useWindowSize()

  return (
    <div className="flex flex-row justify-end gap-2 px-5 items-center">
              <div className="rounded border px-2 sm:hidden text-xs">xs</div>
              <div className="rounded border px-2 hidden sm:block md:hidden text-xs">sm</div>
              <div className="rounded border px-2 hidden md:block lg:hidden text-xs">md</div>
              <div className="rounded border px-2 hidden lg:block xl:hidden text-xs">lg</div>
              <div className="rounded border px-2 hidden xl:block 2xl:hidden text-xs">xl</div>
              <div className="rounded border px-2 hidden 2xl:block 3xl:hidden text-xs">2xl</div>
              <div className="rounded border px-2 hidden 3xl:block 4xl:hidden text-xs">3xl</div>
              <div className="rounded border px-2 hidden 4xl:block 5xl:hidden text-xs">4xl</div>
              <div className="text-xs"> | </div>
              <div className="rounded border px-2 vsm:hidden text-xs">vxs</div>
              <div className="rounded border px-2 hidden vsm:block vlg:hidden text-xs">vsm</div>
              <div className="rounded border px-2 hidden vlg:block vxl:hidden text-xs">vlg</div>
              <div className="rounded border px-2 hidden vxl:block text-xs">vxl</div>
              <div className="text-muted-foreground text-xs">
                {windowSize.width}x{windowSize.height}
              </div>
              <div className="text-muted-foreground text-xs">
                MobiFlight 2025
              </div>
              <div className="text-muted-foreground text-xs">Version 1.0.0</div>
            </div>
  )
}

export default DebugInfo