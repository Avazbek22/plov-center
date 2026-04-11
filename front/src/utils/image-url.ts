export function imageUrl(path: string | null | undefined): string | null {
  if (!path) return null
  return path.startsWith('/') ? path : `/uploads/${path}`
}
