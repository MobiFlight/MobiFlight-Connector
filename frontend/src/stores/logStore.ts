import { create } from 'zustand';
import * as Types from '../types/index';

interface LogMessageState {
    messages: Types.ILogMessage[]
    addMessage: (message: Types.ILogMessage) => void
    clearMessages: () => void
}

export const useLogMessageStore = create<LogMessageState>((set) => ({
    messages: [],
    addMessage: (message) => set((state) => ({ messages : [ ...state.messages, message ] })),
    clearMessages: () => set(() => ({ messages : [] }))
}))