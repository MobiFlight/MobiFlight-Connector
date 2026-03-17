export interface CommunityPost {
  title: string
  tags: string[]
  date: string
  content: string[]
  featured?: boolean
  action?: {
    title: string
    url: string
  }
  media?: {
    type: "image" | "video"
    src: string
    alt: string
    className?: string
  }
}