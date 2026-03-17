import { Button } from "@/components/ui/button"
import { useTranslation } from "react-i18next"
import { useSearchParams } from "react-router"

const CommunityFeedFilter = () => {
  const [searchParams, setSearchParams] = useSearchParams()
  const { t } = useTranslation()
  const activeFilter = searchParams.get("feed_filter") || "all"

  const handleFilterChange = (filter: string) => {
    const newParams = new URLSearchParams(searchParams)
    newParams.set("feed_filter", filter)
    setSearchParams(newParams)
  }

  const className = "h-8 px-3 text-sm"
  const allActive = activeFilter === "all"
  const communityActive = activeFilter === "community"
  const shopActive = activeFilter === "shop"
  const eventsActive = activeFilter === "events"

  return (
    <div
      className="flex flex-row gap-2"
      data-testid="community-feed-filter-bar"
    >
      <Button
        className={className}
        variant={allActive ? "default" : "outline"}
        onClick={() => handleFilterChange("all")}
      >
        {t("Feed.Filter.all")}
      </Button>
      <Button
        className={className}
        variant={communityActive ? "default" : "outline"}
        onClick={() => handleFilterChange("community")}
      >
        {t("Feed.Filter.community")}
      </Button>
      <Button
        className={className}
        variant={shopActive ? "default" : "outline"}
        onClick={() => handleFilterChange("shop")}
      >
        {t("Feed.Filter.shop")}
      </Button>
      <Button
        className={className}
        variant={eventsActive ? "default" : "outline"}
        onClick={() => handleFilterChange("events")}
      >
        {t("Feed.Filter.events")}
      </Button>
    </div>
  )
}

export default CommunityFeedFilter
