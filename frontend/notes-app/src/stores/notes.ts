import { defineStore } from 'pinia';
import { ref, computed } from 'vue';
import { notesService } from '@/services/notes.service';
import type { Note, CreateNoteRequest, UpdateNoteRequest } from '@/types';

export const useNotesStore = defineStore('notes', () => {
  const notes = ref<Note[]>([]);
  const isLoading = ref(false);
  const error = ref<string | null>(null);
  const searchQuery = ref('');
  const sortBy = ref<'date' | 'title'>('date');
  const sortOrder = ref<'asc' | 'desc'>('desc');

  // Pagination
  const currentPage = ref(1);
  const pageSize = ref(9); // 3x3 grid
  const totalCount = ref(0);
  const totalPages = computed(() =>
    Math.ceil(totalCount.value / pageSize.value)
  );

  // Display notes (no client-side filtering with pagination)
  const displayNotes = computed(() => notes.value);

  async function fetchNotes(page?: number) {
    isLoading.value = true;
    error.value = null;

    try {
      if (page) currentPage.value = page;

      const response = await notesService.getPaginated(
        currentPage.value,
        pageSize.value
      );
      notes.value = response.items;
      totalCount.value = response.totalCount;
    } catch (err: any) {
      error.value = err.response?.data?.message || 'Failed to load notes';
    } finally {
      isLoading.value = false;
    }
  }

  function nextPage() {
    if (currentPage.value < totalPages.value) {
      fetchNotes(currentPage.value + 1);
    }
  }

  function previousPage() {
    if (currentPage.value > 1) {
      fetchNotes(currentPage.value - 1);
    }
  }

  function goToPage(page: number) {
    if (page >= 1 && page <= totalPages.value) {
      fetchNotes(page);
    }
  }

  async function createNote(noteData: CreateNoteRequest) {
    error.value = null;

    try {
      const newNote = await notesService.create(noteData);
      notes.value.unshift(newNote);
      return newNote;
    } catch (err: any) {
      error.value = err.response?.data?.message || 'Failed to create note';
      throw err;
    }
  }

  async function updateNote(id: number, noteData: UpdateNoteRequest) {
    error.value = null;

    try {
      const updatedNote = await notesService.update(id, noteData);
      const index = notes.value.findIndex(n => n.id === id);
      if (index !== -1) {
        notes.value[index] = updatedNote;
      }
      return updatedNote;
    } catch (err: any) {
      error.value = err.response?.data?.message || 'Failed to update note';
      throw err;
    }
  }

  async function deleteNote(id: number) {
    error.value = null;

    try {
      await notesService.delete(id);
      notes.value = notes.value.filter(n => n.id !== id);
    } catch (err: any) {
      error.value = err.response?.data?.message || 'Failed to delete note';
      throw err;
    }
  }

  function setSearchQuery(query: string) {
    searchQuery.value = query;
  }

  function setSorting(by: 'date' | 'title', order: 'asc' | 'desc') {
    sortBy.value = by;
    sortOrder.value = order;
  }

  return {
    notes,
    displayNotes,
    isLoading,
    error,
    searchQuery,
    sortBy,
    sortOrder,
    currentPage,
    pageSize,
    totalCount,
    totalPages,
    fetchNotes,
    createNote,
    updateNote,
    deleteNote,
    setSearchQuery,
    setSorting,
    nextPage,
    previousPage,
    goToPage
  };
});
