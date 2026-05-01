export type MembershipStatus = "basic" | "member"
export type SubscriptionStatus = "active" | "canceled"

export interface SubcriptionStatus {
  createdAt: Date
  updatedAt: Date
  status: SubscriptionStatus
  stripeId: string
}

export interface Profile {
  _id: string
  userId: string
  membership: MembershipStatus
  subscription?: SubcriptionStatus
  createdAt: Date
  updatedAt: Date
}