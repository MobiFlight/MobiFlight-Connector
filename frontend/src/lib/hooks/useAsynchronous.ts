import { SaveStatus, useProjectStore } from "@/stores/projectStore"

export function useAsynchronous() {
  const waitForSaveStatus = (timeout = 30000): Promise<SaveStatus> => {
    const finalStates = ["success", "error", "cancelled"]
    const currentStatus = useProjectStore.getState().saveStatus

    // return immediately if already in a final state
    // otherwise, subscription will timeout
    if (finalStates.includes(currentStatus)) {
      return Promise.resolve(currentStatus)
    }

    return new Promise((resolve, reject) => {
      const unsubscribe = useProjectStore.subscribe((state) => {
        const status = state.saveStatus
        // Only resolve on final states
        if (finalStates.includes(status)) {
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
