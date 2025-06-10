import { useRef } from 'react'

function useIsFirstRender() {
    const isFirstRenderRef = useRef(true)

    if (isFirstRenderRef.current) {
        isFirstRenderRef.current = false
        return true
    }

    return isFirstRenderRef.current
}

export default useIsFirstRender