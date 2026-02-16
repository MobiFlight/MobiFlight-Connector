import { useOutlet } from "react-router"

const Plain = () => {
  const outlet = useOutlet()
  return (
    <>
      <div className="flex flex-row items-center justify-center fixed inset-0 bg-linear-to-br from-indigo-500 from-10% via-sky-500 via-30% to-emerald-500 to-90% dark:from-indigo-500/10 dark:via-sky-500/0 dark:via-70% dark:to-emerald-500/5">
        {outlet}
      </div>
    </>
  )
}
export default Plain