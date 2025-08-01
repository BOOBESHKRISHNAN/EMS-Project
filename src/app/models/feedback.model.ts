export interface Feedback {
  id: number;
  eventId: number;
  userId: number;
  ticketId: number;
  rating: number;
  comment?: string;
  createdAt: Date;
}

export interface FeedbackCreateDTO {
  eventId: number;
  ticketId: number;
  rating: number;
  comment?: string;
}

export interface FeedbackResponseDTO {
  id: number;
  eventId: number;
  userId: number;
  ticketId: number;
  rating: number;
  comment?: string;
  createdAt: string;
  userName: string;
}

export interface FeedbackSummaryDTO {
  eventId: number;
  averageRating: number;
  totalFeedbacks: number;
  feedbacks: FeedbackResponseDTO[];
}