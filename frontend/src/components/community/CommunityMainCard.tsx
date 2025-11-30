import IconBrandMobiFlightLogo from "@/components/icons/IconBrandMobiFlightLogo"
import { Button } from "@/components/ui/button"
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card"
import { cn } from "@/lib/utils"

const CommunityMainCard = () => {
  const communityFeed = [
    {
      title: "MobiFlight v11 Released!",
      date: "2024-05-01",
      content:
        "We're excited to announce the release of MobiFlight v11, featuring new controller support and enhanced configuration options.",
    },
    {
      title: "Upcoming Webinar: Getting Started with MobiFlight",
      date: "2024-04-20",
      content:
        "Join us for a live webinar where we'll walk you through the basics of setting up MobiFlight and answer your questions.",
      featured: true,
      action: {
        title: "Register Now",
        url: "https://mobiflight.com/webinar",
      },
    },
    {
      title: "Join the MobiFlight Forum",
      date: "2024-04-15",
      content:
        "Connect with other MobiFlight users, share your projects, and get help on our official forum.",
    },
    {
      title: "New Tutorial: Setting Up MobiFlight with msfs",
      date: "2024-04-10",
      content:
        "Check out our latest tutorial on how to set up MobiFlight with Microsoft Flight Simulator 2020 for an immersive experience.",
    },
  ]

  return (
    <Card className="border-shadow-none bg-muted rounded-none h-full">
      <CardHeader>
        <CardTitle className="flex flex-row gap-2"><IconBrandMobiFlightLogo /> Community Feed</CardTitle>
        <CardDescription>
          News and updates from the MobiFlight community.
        </CardDescription>
      </CardHeader>
      <CardContent>
        <div className="flex flex-col gap-4">
          {communityFeed.map((post) => (
            <div
              key={post.title}
              className={cn(
                "border-muted border-b p-4",
                post.featured && "bg-background rounded-md",
              )}
            >
              <h4 className="font-semibold">{post.title}</h4>
              <span className="text-muted-foreground text-sm">{post.date}</span>
              <p className="text-sm">{post.content}</p>
              {post.action && (
                <div className="mt-2">
                  <Button
                    size={"sm"}
                    onClick={() => window.open(post.action!.url, "_blank")}
                  >
                    {post.action!.title}
                  </Button>
                </div>
              )}
            </div>
          ))}
        </div>
      </CardContent>
    </Card>
  )
}

export default CommunityMainCard
