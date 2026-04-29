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
import { fetchRemoteCommunityFeed } from "@/lib/feed"
import { useErrorFallbackTest } from "@/lib/hooks/useErrorFallbackTest"
import { CommunityPost } from "@/types/feed"
import { useQuery } from "@tanstack/react-query"
import { useTranslation } from "react-i18next"
import { useSearchParams } from "react-router"

const CommunityMainCard = () => {
  // this component is wrapped in an error boundary
  // so we can trigger errors for testing purposes here
  const { trigger } = useErrorFallbackTest()
  trigger("community-main-card")

  const [searchParams] = useSearchParams()
  const activeFilter = searchParams.get("feed_filter") || "all"
  const { t, i18n } = useTranslation()

  const communityFeed = t("feed:community", {
    returnObjects: true,
  }) as CommunityPost[]

  
  // For local development, we are able to point the feed to a custom url 
  // This url is optionally defined in the environment variable VITE_FEED_REMOTE_BASE_URL
  // If not set, the frontend will default to "https://mobiflight.com/feed" as the base url for the feed
  const remoteFeedDefaultBaseUrl = "https://mobiflight.com/feed"
  const remoteFeedBaseUrl = (import.meta.env.VITE_FEED_REMOTE_BASE_URL ?? remoteFeedDefaultBaseUrl).trim()

  const language = i18n.resolvedLanguage || i18n.language || "en"

  const remoteFeedQuery = useQuery({
    queryKey: ["community-feed", language, remoteFeedBaseUrl],
    queryFn: () =>
      fetchRemoteCommunityFeed({
        baseUrl: remoteFeedBaseUrl,
        language
      }),
  })

  const displayedFeed = [ ...remoteFeedQuery.data ?? [], ...communityFeed ]

  const filteredFeed = displayedFeed.filter(
    (post) => post.tags.includes(activeFilter) || activeFilter === "all",
  )

  return (
    <Card
      className="border-shadow-none bg-muted flex h-full flex-col rounded-none"
      data-testid="community-main-card"
    >
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
