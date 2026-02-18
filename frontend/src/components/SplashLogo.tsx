import logo from "../assets/mobiflight-logo-border.png"

const SplashLogo = () => {
  return (
    <div className="animate-low-bounce">
      <img
        src={logo}
        alt="MobiFlight Logo"
        className="pointer-events-none h-36 w-36 shadow-xl shadow-slate-800/40"
      />
    </div>
  )
}
export default SplashLogo
