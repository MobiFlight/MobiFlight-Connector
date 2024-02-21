import React from "react"
type Headline2Props = {
  children: React.ReactNode
}

const Headline2 = ({children} : Headline2Props) : React.ReactNode => {
  return (
    <h2 className="pb-8 text-3xl font-semibold tracking-tight first:mt-0">{children}</h2>
  )
}

export default Headline2