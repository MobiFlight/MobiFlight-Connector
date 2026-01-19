import { SaveStatus, useProjectStore } from "@/stores/projectStore"

export function useAsynchonous() {
  const waitForSaveStatus = (timeout = 30000): Promise<SaveStatus> => {
    return new Promise((resolve, reject) => {
      const unsubscribe = useProjectStore.subscribe((state) => {
        const status = state.saveStatus
        // Only resolve on terminal states
        if (["success", "error", "cancelled"].includes(status)) {
          unsubscribe()
          clearTimeout(timer)
          resolve(status)
        }
      })
      
      const timer = setTimeout(() => {
        unsubscribe()
        reject(new Error("Save timeout"))
      }, timeout)
    })
  }

  return { waitForSaveStatus }
}
