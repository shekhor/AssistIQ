import { useMemo, useState } from 'react'
import {
  ApiError,
  createTicket,
  getRagSuggestions,
  generateAiResponse,
  submitFeedback,
  submitVerification,
} from './api'
import type { AiGenerateResponse, RagSuggestion } from './types'

const feedbackOptions = ['Correct', 'Corrected', 'Rejected'] as const

function App() {
  const [externalSystem, setExternalSystem] = useState('Halo')
  const [externalTicketRef, setExternalTicketRef] = useState('')
  const [ticketText, setTicketText] = useState('')
  const [ticketId, setTicketId] = useState<number | null>(null)
  const [aiResponse, setAiResponse] = useState<AiGenerateResponse | null>(null)
  const [ragSuggestions, setRagSuggestions] = useState<RagSuggestion[]>([])
  const [isLoadingRag, setIsLoadingRag] = useState(false)
  const [ragError, setRagError] = useState<string | null>(null)
  const [verifiedSolutionText, setVerifiedSolutionText] = useState('')
  const [feedbackSolutionText, setFeedbackSolutionText] = useState('')
  const [verifiedBy, setVerifiedBy] = useState('')
  const [feedbackType, setFeedbackType] =
    useState<(typeof feedbackOptions)[number]>('Corrected')
  const [solutionId, setSolutionId] = useState<number | null>(null)
  const [isSubmitting, setIsSubmitting] = useState(false)
  const [isVerifying, setIsVerifying] = useState(false)
  const [isSubmittingFeedback, setIsSubmittingFeedback] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const [status, setStatus] = useState<string | null>(null)
  const [feedbackSent, setFeedbackSent] = useState(false)

  const hasTicket = ticketId !== null
  const hasAiResponse = aiResponse !== null
  const isReadyForActions = hasTicket && hasAiResponse

  const confidenceLabel = useMemo(() => {
    if (!aiResponse) return ''
    const percent = Math.round(aiResponse.confidenceScore * 100)
    return `${percent}% confidence`
  }, [aiResponse])

  const resetWorkflow = () => {
    setTicketId(null)
    setAiResponse(null)
    setRagSuggestions([])
    setIsLoadingRag(false)
    setRagError(null)
    setVerifiedSolutionText('')
    setFeedbackSolutionText('')
    setVerifiedBy('')
    setFeedbackType('Corrected')
    setSolutionId(null)
    setFeedbackSent(false)
  }

  const getErrorMessage = (err: unknown) => {
    if (err instanceof ApiError) {
      return err.message
    }

    if (err instanceof Error) {
      return err.message
    }

    return 'Something went wrong. Please try again.'
  }

  const handleError = (err: unknown) => {
    setError(getErrorMessage(err))
  }

  const handleSubmitTicket = async (event: React.FormEvent) => {
    event.preventDefault()
    if (!ticketText.trim()) {
      setError('Ticket description is required.')
      return
    }

    setIsSubmitting(true)
    setError(null)
    setStatus(null)
    resetWorkflow()

    try {
      const ticket = await createTicket({
        externalSystem: externalSystem.trim(),
        externalTicketRef: externalTicketRef.trim(),
        ticketText: ticketText.trim(),
      })
      setTicketId(ticket.ticketId)

      setIsLoadingRag(true)
      setRagError(null)
      getRagSuggestions(ticket.ticketId)
        .then((suggestions) => setRagSuggestions(suggestions))
        .catch((err) => setRagError(getErrorMessage(err)))
        .finally(() => setIsLoadingRag(false))

      const response = await generateAiResponse(ticket.ticketId)
      setAiResponse(response)
      setVerifiedSolutionText(response.responseText)
      setStatus('AI suggestion generated. Review below to confirm or correct.')
    } catch (err) {
      handleError(err)
    } finally {
      setIsSubmitting(false)
    }
  }

  const handleVerify = async () => {
    if (!ticketId) return
    if (!verifiedSolutionText.trim()) {
      setError('Verified solution text is required to confirm the answer.')
      return
    }

    setIsVerifying(true)
    setError(null)
    setStatus(null)

    try {
      const result = await submitVerification(ticketId, {
        verifiedSolutionText: verifiedSolutionText.trim(),
      })
      setSolutionId(result.solutionId)
      setStatus('Verified solution saved.')
    } catch (err) {
      handleError(err)
    } finally {
      setIsVerifying(false)
    }
  }

  const handleSubmitFeedback = async () => {
    if (!ticketId || !aiResponse) return
    const trimmedFeedbackSolution = feedbackSolutionText.trim()
    if (feedbackType !== 'Correct' && !trimmedFeedbackSolution) {
      setError('Corrected solution text is required for this feedback type.')
      return
    }
    if (!verifiedBy.trim()) {
      setError('Verified by is required to submit feedback.')
      return
    }

    setIsSubmittingFeedback(true)
    setError(null)
    setStatus(null)

    try {
      const verifiedSolutionTextForFeedback =
        feedbackType === 'Correct'
          ? aiResponse.responseText
          : trimmedFeedbackSolution

      const result = await submitVerification(ticketId, {
        verifiedSolutionText: verifiedSolutionTextForFeedback,
      })

      await submitFeedback({
        responseId: aiResponse.responseId,
        verifiedSolutionId: result.solutionId,
        feedbackType,
        verifiedBy: verifiedBy.trim(),
      })

      setSolutionId(result.solutionId)
      setFeedbackSent(true)
      setStatus('Feedback submitted and verified solution saved.')
    } catch (err) {
      handleError(err)
    } finally {
      setIsSubmittingFeedback(false)
    }
  }

  return (
    <div className="page">
      <header className="hero">
        <div>
          <p className="eyebrow">AssistIQ AI</p>
          <h1>Ticket AI Suggestion Workflow</h1>
          <p className="subtitle">
            Submit a ticket description, review the AI response, and confirm or
            correct the solution.
          </p>
        </div>
        <div className="meta">
          <span className="pill">API base: /api</span>
          {ticketId !== null ? (
            <span className="pill">Ticket #{ticketId}</span>
          ) : null}
        </div>
      </header>

      <main className="grid">
        <section className="card">
          <h2>Create Ticket</h2>
          <form className="form" onSubmit={handleSubmitTicket}>
            <label>
              External system
              <input
                type="text"
                value={externalSystem}
                onChange={(event) => setExternalSystem(event.target.value)}
                placeholder="Halo"
              />
            </label>
            <label>
              External ticket reference
              <input
                type="text"
                value={externalTicketRef}
                onChange={(event) => setExternalTicketRef(event.target.value)}
                placeholder="ABC-123"
              />
            </label>
            <label>
              Ticket description
              <textarea
                value={ticketText}
                onChange={(event) => setTicketText(event.target.value)}
                placeholder="Describe the issue from the external system..."
                rows={6}
                required
              />
            </label>
            <div className="actions">
              <button type="submit" disabled={isSubmitting}>
                {isSubmitting ? 'Submitting...' : 'Submit & generate AI'}
              </button>
              {hasAiResponse ? (
                <button
                  type="button"
                  className="secondary"
                  onClick={resetWorkflow}
                >
                  Reset workflow
                </button>
              ) : null}
            </div>
          </form>
        </section>

        <section className="card">
          <h2>AI Suggestion</h2>
          {hasAiResponse ? (
            <div className="response">
              <div className="response-header">
                <span className="pill">Response #{aiResponse.responseId}</span>
                <span className="pill">{confidenceLabel}</span>
              </div>
              <p className="response-text">{aiResponse.responseText}</p>
              <label>
                Verified solution (for confirmation)
                <textarea
                  value={verifiedSolutionText}
                  onChange={(event) =>
                    setVerifiedSolutionText(event.target.value)
                  }
                  rows={4}
                />
              </label>
              <button
                type="button"
                onClick={handleVerify}
                disabled={!isReadyForActions || isVerifying}
              >
                {isVerifying ? 'Saving...' : 'Confirm solution'}
              </button>
              {solutionId !== null ? (
                <p className="hint">Saved verified solution #{solutionId}.</p>
              ) : null}
            </div>
          ) : (
            <p className="muted">
              Submit a ticket to generate the AI suggestion.
            </p>
          )}
          <div className="rag">
            <h3>Similar verified solutions</h3>
            {hasTicket ? (
              <>
                {isLoadingRag ? (
                  <p className="muted">Loading similar solutions...</p>
                ) : ragError ? (
                  <p className="error">{ragError}</p>
                ) : ragSuggestions.length > 0 ? (
                  <ol className="rag-list">
                    {ragSuggestions.map((suggestion) => (
                      <li key={suggestion.solutionId} className="rag-item">
                        <p>{suggestion.verifiedSolutionText}</p>
                        {typeof suggestion.similarityScore === 'number' ? (
                          <span className="pill">
                            Similarity{' '}
                            {Math.round(suggestion.similarityScore * 100)}%
                          </span>
                        ) : null}
                      </li>
                    ))}
                  </ol>
                ) : (
                  <p className="muted">No similar verified solutions yet.</p>
                )}
              </>
            ) : (
              <p className="muted">
                Create a ticket to load similar verified solutions.
              </p>
            )}
          </div>
        </section>

      <section className="card">
          <h2>Feedback & Correction</h2>
          {hasAiResponse ? (
            <div className="form">
              <label>
                Verified solution for feedback
                <textarea
                  value={feedbackSolutionText}
                  onChange={(event) =>
                    setFeedbackSolutionText(event.target.value)
                  }
                  rows={5}
                  placeholder="Provide the verified solution if the AI needs correction."
                  disabled={feedbackType === 'Correct'}
                />
              </label>
              {feedbackType === 'Correct' ? (
                <p className="muted">
                  Using the AI response as the verified solution.
                </p>
              ) : null}
              <label>
                Verified by
                <input
                  type="email"
                  value={verifiedBy}
                  onChange={(event) => setVerifiedBy(event.target.value)}
                  placeholder="agent@city.ca"
                />
              </label>
              <label>
                Feedback type
                <select
                  value={feedbackType}
                  onChange={(event) =>
                    setFeedbackType(
                      event.target.value as (typeof feedbackOptions)[number]
                    )
                  }
                >
                  {feedbackOptions.map((option) => (
                    <option key={option} value={option}>
                      {option}
                    </option>
                  ))}
                </select>
              </label>
              <p className="muted">
                Confidence updates: Correct → 1.0, Corrected → 0.8, Rejected → 0.2
              </p>
              <button
                type="button"
                onClick={handleSubmitFeedback}
                disabled={!isReadyForActions || isSubmittingFeedback}
              >
                {isSubmittingFeedback ? 'Submitting...' : 'Submit feedback'}
              </button>
              {feedbackSent ? (
                <p className="hint">
                  Feedback sent. Verified solution #{solutionId} linked.
                </p>
              ) : null}
            </div>
          ) : (
            <p className="muted">
              Generate an AI suggestion before submitting feedback.
            </p>
          )}
        </section>
      </main>

      {(error || status) && (
        <section className="notice">
          {error ? <p className="error">{error}</p> : null}
          {status ? <p className="status">{status}</p> : null}
        </section>
      )}
    </div>
  )
}

export default App


