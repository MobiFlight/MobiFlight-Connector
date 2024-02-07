import React from 'react'

type SidebarProps = {
    className? : string
}

const Sidebar = (props: SidebarProps) => {
  return (
    <div className={`${props.className} text-primary-foreground bg-primary w-16`}>Sidebar</div>
  )
}

export default Sidebar