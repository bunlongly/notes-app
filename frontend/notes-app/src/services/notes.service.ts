import api from './api';
import type { Note, CreateNoteRequest, UpdateNoteRequest } from '@/types';

interface PaginatedResponse {
  items: Note[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}

export const notesService = {
  async getAll(): Promise<Note[]> {
    const response = await api.get<Note[]>('/notes');
    return response.data;
  },

  async getPaginated(
    page: number,
    pageSize: number
  ): Promise<PaginatedResponse> {
    const response = await api.get<PaginatedResponse>('/notes', {
      params: { page, pageSize }
    });
    return response.data;
  },

  async getById(id: number): Promise<Note> {
    const response = await api.get<Note>(`/notes/${id}`);
    return response.data;
  },

  async create(note: CreateNoteRequest): Promise<Note> {
    const response = await api.post<Note>('/notes', note);
    return response.data;
  },

  async update(id: number, note: UpdateNoteRequest): Promise<Note> {
    const response = await api.put<Note>(`/notes/${id}`, note);
    return response.data;
  },

  async delete(id: number): Promise<void> {
    await api.delete(`/notes/${id}`);
  }
};
