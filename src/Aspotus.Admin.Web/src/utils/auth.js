function parseJwtPayload(token) {
  try {
    const payload = token.split('.')[1]
    if (!payload) return null
    const base64 = payload.replace(/-/g, '+').replace(/_/g, '/')
    const padded = base64.padEnd(Math.ceil(base64.length / 4) * 4, '=')
    return JSON.parse(atob(padded))
  } catch {
    return null
  }
}

export function getUserRoles() {
  const token = localStorage.getItem('token')
  if (!token) return []

  const payload = parseJwtPayload(token)
  if (!payload) return []

  const roleClaim = payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role']
  if (!roleClaim) return []
  return Array.isArray(roleClaim) ? roleClaim : [roleClaim]
}

export function isOperator() {
  return getUserRoles().includes('Operator')
}

export function isAdmin() {
  return getUserRoles().includes('Admin')
}

export function isContentModerator() {
  return getUserRoles().includes('ContentModerator')
}

export function isUsersSectionBlocked() {
  return !isAdmin()
}
