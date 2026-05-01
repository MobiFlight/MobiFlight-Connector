import { Profile } from "@/types/profile"
import {create} from "zustand"

interface UserProfileState {
  userProfile: Profile | null
  setUserProfile: (profile: Profile | null) => void
}

export const useUserProfileStore = create<UserProfileState>((set) => ({
  userProfile: null,
  setUserProfile: (profile) => set({ userProfile: profile }),
}))
