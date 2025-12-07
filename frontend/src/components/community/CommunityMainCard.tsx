import CommunityFeedFilter from "@/components/community/CommunityFeedFilter"
import CommunityFeedItem from "@/components/community/CommunityFeedItem"
import IconBrandMobiFlightLogo from "@/components/icons/IconBrandMobiFlightLogo"

import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card"
import { ScrollArea } from "@/components/ui/scroll-area"
import { CommunityPost } from "@/types/feed"
import { useTranslation } from "react-i18next"
import { useSearchParams } from "react-router"

const CommunityMainCard = () => {
  const [searchParams] = useSearchParams()
  const activeFilter = searchParams.get("feed_filter") || "all"
  const { t } = useTranslation()

  const communityFeed = t("feed:community", {
    returnObjects: true,
  }) as CommunityPost[]

  const filteredFeed = communityFeed.filter(
    (post) => post.tags.includes(activeFilter) || activeFilter === "all",
  )

  return (
    <Card className="border-shadow-none bg-muted flex h-full flex-col rounded-none">
      <CardHeader>
        <CardTitle className="flex flex-row gap-2">
          <IconBrandMobiFlightLogo /> {t("Feed.Title")}
        </CardTitle>
        <CardDescription>{t("Feed.Description")}</CardDescription>
      </CardHeader>
      <CardContent className="flex grow flex-col gap-8">
        <CommunityFeedFilter />
        <ScrollArea className="grow">
          <div className="flex flex-col gap-8 pr-4">
            {filteredFeed.map((post, index) => (
              <div key={index} className="flex flex-col gap-8">
                <CommunityFeedItem post={post} />
                <div className="border" />
              </div>
            ))}
            {filteredFeed.length === 0 && (
              <p className="text-muted-foreground text-md bg-background rounded-md border p-8 text-center">
                {t("Feed.NoPosts")}
              </p>
            )}
          </div>
        </ScrollArea>
      </CardContent>
    </Card>
  )
}

export default CommunityMainCard
