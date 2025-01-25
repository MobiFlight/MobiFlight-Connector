import { Outlet } from 'react-router'

function App() {
  return (
    <>
      <div className="flex h-svh flex-row">
        {/* <Sidebar /> */}
        <div className="flex grow flex-col">
          {/* <Navbar /> */}
          <div className="flex grow flex-col overflow-hidden p-12">
            <Outlet />
          </div>
          <div className="flex flex-row justify-end gap-2 bg-white px-5 py-2 dark:bg-zinc-800">
            <div className="text-xs text-gray-500">MobiFlight 2025</div>
            <div className="text-xs text-gray-500">Version 1.0.0</div>
          </div>
        </div>
        {/* <Toaster
          position="bottom-right"
          offset={48}
        /> */}
      </div>
    </>
  )
}

export default App
