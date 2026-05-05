import type {
  AiGenerateResponse,
  FeedbackRequest,
  FeedbackResponse,
  RagSuggestion,
  TicketCreateRequest,
  TicketCreateResponse,
  VerifyRequest,
  VerifyResponse,
} from './types'

const API_BASE = import.meta.env.VITE_API_BASE_URL ?? 'https://localhost:7249/api'

class ApiError extends Error {
  status: number

  constructor(message: string, status: number) {
    super(message)
    this.status = status
  }
}

async function request<TResponse>(
  path: string,
  options: RequestInit = {}
): Promise<TResponse> {
  const response = await fetch(`${API_BASE}${path}`, {
    headers: {
      'Content-Type': 'application/json',
      ...(options.headers ?? {}),
    },
    ...options,
  })

  if (!response.ok) {
    let message = response.statusText
    try {
      const body = await response.json()
      if (body?.message) {
        message = body.message
      }
    } catch {
      // Ignore json parsing errors, fall back to statusText.
    }

    throw new ApiError(message || 'Request failed', response.status)
  }

  if (response.status === 204) {
    return undefined as TResponse
  }

  return (await response.json()) as TResponse
}

export async function createTicket(body: TicketCreateRequest) {
  return request<TicketCreateResponse>('/tickets', {
    method: 'POST',
    body: JSON.stringify(body),
  })
}

export async function generateAiResponse(ticketId: number) {
  return request<AiGenerateResponse>(`/tickets/${ticketId}/ai-generate`, {
    method: 'POST',
  })
}

export async function getRagSuggestions(ticketId: number) {
  return request<RagSuggestion[]>(`/tickets/${ticketId}/rag`)
}

export async function submitVerification(
  ticketId: number,
  body: VerifyRequest
) {
  return request<VerifyResponse>(`/tickets/${ticketId}/verify`, {
    method: 'POST',
    body: JSON.stringify(body),
  })
}

export async function submitFeedback(body: FeedbackRequest) {
  return request<FeedbackResponse>('/feedback', {
    method: 'POST',
    body: JSON.stringify(body),
  })
}

export { ApiError }

