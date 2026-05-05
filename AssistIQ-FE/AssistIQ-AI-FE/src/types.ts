export type Ticket = {
  ticketId: number
  externalSystem: string
  externalTicketRef: string
  ticketText: string
  createdAt: string
}

export type TicketCreateRequest = {
  externalSystem: string
  externalTicketRef: string
  ticketText: string
}

export type TicketCreateResponse = {
  ticketId: number
}

export type AiResponse = {
  responseId: number
  ticketId: number
  responseText: string
  confidenceScore: number
  createdAt: string
}

export type AiGenerateResponse = {
  ticketId: number
  responseId: number
  responseText: string
  confidenceScore: number
}

export type VerifyRequest = {
  verifiedSolutionText: string
}

export type VerifyResponse = {
  ticketId: number
  solutionId: number
}

export type FeedbackRequest = {
  responseId: number
  verifiedSolutionId: number
  feedbackType: string
  verifiedBy: string
}

export type FeedbackResponse = {
  responseId: number
}

export type RagSuggestion = {
  solutionId: number
  verifiedSolutionText: string
  similarityScore?: number
  ticketId?: number
}


