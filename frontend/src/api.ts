export interface Campaign {
  id: string
  name: string
  schoolYear: string
  isDemo: boolean
  programs: { name: string; capacity: number }[]
}

export type ServiceName = 'catalog' | 'applications' | 'ranking'

export async function getJson<T>(path: string, signal: AbortSignal): Promise<T> {
  const response = await fetch(path, { signal, headers: { Accept: 'application/json' } })
  if (!response.ok) throw new Error(`HTTP ${response.status}`)
  return response.json() as Promise<T>
}
