import { SaveStatus, useProjectStore } from "@/stores/projectStore"

export function useAsynchronous() {
  const waitForSaveStatus = (): Promise<SaveStatus> => {
    const finalStates = ["success", "error", "cancelled"]
    const currentStatus = useProjectStore.getState().saveStatus

    // return immediately if already in a final state
    // otherwise, subscription will timeout
    if (finalStates.includes(currentStatus)) {
      return Promise.resolve(currentStatus)
    }

    return new Promise((resolve) => {
      const unsubscribe = useProjectStore.subscribe((state) => {
        const status = state.saveStatus
        // Only resolve on final states
        if (finalStates.includes(status)) {
          unsubscribe()
          resolve(status)
        }
      })
    })
  }

  return { waitForSaveStatus }
}
