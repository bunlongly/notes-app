<template>
  <div class="min-h-screen bg-gray-50">
    <!-- Header -->
    <header class="bg-white shadow-sm">
      <div
        class="max-w-7xl mx-auto px-4 py-4 sm:px-6 lg:px-8 flex justify-between items-center"
      >
        <div>
          <h1 class="text-2xl font-bold text-gray-900">My Notes</h1>
          <p class="text-sm text-gray-600">
            Welcome, {{ authStore.user?.fullName }}
          </p>
        </div>
        <button
          @click="handleLogout"
          class="px-4 py-2 text-sm text-red-600 hover:text-red-700 font-medium"
        >
          Logout
        </button>
      </div>
    </header>

    <div class="max-w-7xl mx-auto px-4 py-8 sm:px-6 lg:px-8">
      <!-- Error Message -->
      <AlertMessage
        v-if="errorMessage"
        :message="errorMessage"
        variant="error"
        class="mb-6"
        @close="errorMessage = null"
      />

      <!-- Search and Actions -->
      <div class="mb-6 flex flex-col sm:flex-row gap-4">
        <div class="flex-1">
          <input
            v-model="notesStore.searchQuery"
            type="text"
            placeholder="Search notes..."
            class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500 outline-none"
          />
        </div>

        <div class="flex gap-2">
          <select
            v-model="sortBy"
            @change="handleSortChange"
            class="px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500 outline-none"
          >
            <option value="date">Sort by Date</option>
            <option value="title">Sort by Title</option>
          </select>

          <button
            @click="toggleSortOrder"
            class="px-4 py-2 border border-gray-300 rounded-lg hover:bg-gray-50"
            :title="sortOrder === 'desc' ? 'Descending' : 'Ascending'"
          >
            {{ sortOrder === 'desc' ? '↓' : '↑' }}
          </button>

          <button
            @click="openCreateModal"
            class="px-6 py-2 bg-indigo-600 text-white rounded-lg hover:bg-indigo-700 font-medium"
          >
            + New Note
          </button>
        </div>
      </div>

      <!-- Notes Grid -->
      <div v-if="notesStore.isLoading" class="text-center py-12">
        <p class="text-gray-500">Loading notes...</p>
      </div>

      <div v-else-if="notesStore.notes.length === 0" class="text-center py-12">
        <p class="text-gray-500 text-lg">
          No notes yet. Create your first note!
        </p>
      </div>

      <div v-else class="space-y-6">
        <!-- Notes grid -->
        <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
          <div
            v-for="note in notesStore.notes"
            :key="note.id"
            class="bg-white rounded-lg shadow-md p-6 hover:shadow-lg transition-shadow cursor-pointer"
            @click="openViewModal(note)"
          >
            <h3 class="text-lg font-semibold text-gray-800 mb-2 truncate">
              {{ note.title }}
            </h3>
            <p class="text-gray-600 text-sm mb-4 line-clamp-3">
              {{ note.content || 'No content' }}
            </p>
            <div class="text-xs text-gray-500 mb-3 space-y-1">
              <div>Created: {{ formatTimestamp(note.createdAt) }}</div>
              <div>Updated: {{ formatTimestamp(note.updatedAt) }}</div>
            </div>
            <div
              class="flex justify-end items-center gap-3 pt-3 border-t border-gray-100"
            >
              <button
                @click.stop="openEditModal(note)"
                class="text-indigo-600 hover:text-indigo-700 font-medium"
              >
                Edit
              </button>
              <button
                @click.stop="handleDelete(note.id)"
                class="text-red-600 hover:text-red-700 font-medium"
              >
                Delete
              </button>
            </div>
          </div>
        </div>

        <!-- Pagination Controls -->
        <div
          v-if="notesStore.totalPages > 1"
          class="flex justify-center items-center gap-2"
        >
          <button
            @click="notesStore.previousPage()"
            :disabled="notesStore.currentPage === 1"
            :class="[
              'px-4 py-2 rounded-lg font-medium transition-colors',
              notesStore.currentPage === 1
                ? 'bg-gray-100 text-gray-400 cursor-not-allowed'
                : 'bg-white text-indigo-600 hover:bg-indigo-50 border border-indigo-600'
            ]"
          >
            Previous
          </button>

          <div class="flex gap-1">
            <button
              v-for="page in paginationPages"
              :key="page"
              @click="notesStore.goToPage(page)"
              :class="[
                'w-10 h-10 rounded-lg font-medium transition-colors',
                page === notesStore.currentPage
                  ? 'bg-indigo-600 text-white'
                  : 'bg-white text-gray-700 hover:bg-gray-100 border border-gray-300'
              ]"
            >
              {{ page }}
            </button>
          </div>

          <button
            @click="notesStore.nextPage()"
            :disabled="notesStore.currentPage === notesStore.totalPages"
            :class="[
              'px-4 py-2 rounded-lg font-medium transition-colors',
              notesStore.currentPage === notesStore.totalPages
                ? 'bg-gray-100 text-gray-400 cursor-not-allowed'
                : 'bg-white text-indigo-600 hover:bg-indigo-50 border border-indigo-600'
            ]"
          >
            Next
          </button>
        </div>

        <!-- Page info -->
        <div class="text-center text-sm text-gray-600">
          Showing page {{ notesStore.currentPage }} of
          {{ notesStore.totalPages }} ({{ notesStore.totalCount }} total notes)
        </div>
      </div>
    </div>

    <!-- Create/Edit Modal -->
    <div
      v-if="showModal"
      class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center p-4 z-50"
      @click.self="closeModal"
    >
      <div class="bg-white rounded-lg max-w-2xl w-full p-6">
        <h2 class="text-2xl font-bold mb-4">
          {{
            modalMode === 'create'
              ? 'Create Note'
              : modalMode === 'edit'
              ? 'Edit Note'
              : 'View Note'
          }}
        </h2>

        <AlertMessage
          v-if="errorMessage"
          :message="errorMessage"
          variant="error"
          class="mb-4"
          @close="errorMessage = null"
        />

        <div class="space-y-4">
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1"
              >Title *</label
            >
            <input
              v-model="currentNote.title"
              :readonly="modalMode === 'view'"
              type="text"
              class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500"
              placeholder="Note title"
            />
          </div>

          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1"
              >Content</label
            >
            <textarea
              v-model="currentNote.content"
              :readonly="modalMode === 'view'"
              rows="10"
              class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500"
              placeholder="Write your note here..."
            ></textarea>
          </div>

          <div
            v-if="modalMode === 'view' && currentNote.createdAt"
            class="text-sm text-gray-600 space-y-1 pt-2 border-t border-gray-200"
          >
            <div>
              <strong>Created:</strong>
              {{ formatTimestamp(currentNote.createdAt) }}
            </div>
            <div>
              <strong>Last updated:</strong>
              {{ formatTimestamp(currentNote.updatedAt) }}
            </div>
          </div>
        </div>

        <div class="mt-6 flex justify-end gap-3">
          <button
            @click="closeModal"
            class="px-4 py-2 border border-gray-300 rounded-lg hover:bg-gray-50"
          >
            {{ modalMode === 'view' ? 'Close' : 'Cancel' }}
          </button>

          <button
            v-if="modalMode !== 'view'"
            @click="handleSave"
            class="px-4 py-2 bg-indigo-600 text-white rounded-lg hover:bg-indigo-700"
          >
            {{ modalMode === 'create' ? 'Create' : 'Save' }}
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, computed } from 'vue';
import { useRouter } from 'vue-router';
import { useAuthStore } from '@/stores/auth';
import { useNotesStore } from '@/stores/notes';
import type { Note } from '@/types';
import { formatTimestamp } from '@/utils/dateFormat';
import AlertMessage from '@/components/AlertMessage.vue';

const router = useRouter();
const authStore = useAuthStore();
const notesStore = useNotesStore();

const showModal = ref(false);
const modalMode = ref<'create' | 'edit' | 'view'>('create');
const currentNote = ref({
  id: 0,
  title: '',
  content: '',
  createdAt: '',
  updatedAt: ''
});

const sortBy = ref<'date' | 'title'>('date');
const sortOrder = ref<'asc' | 'desc'>('desc');
const errorMessage = ref<string | null>(null);

const paginationPages = computed(() => {
  const pages = [];
  const total = notesStore.totalPages;
  const current = notesStore.currentPage;

  // Show max 5 pages at a time
  let start = Math.max(1, current - 2);
  let end = Math.min(total, start + 4);

  // Adjust start if we're near the end
  if (end - start < 4) {
    start = Math.max(1, end - 4);
  }

  for (let i = start; i <= end; i++) {
    pages.push(i);
  }

  return pages;
});

onMounted(() => {
  notesStore.fetchNotes();
});

const handleLogout = () => {
  authStore.logout();
  router.push('/login');
};

const openCreateModal = () => {
  modalMode.value = 'create';
  currentNote.value = {
    id: 0,
    title: '',
    content: '',
    createdAt: '',
    updatedAt: ''
  };
  showModal.value = true;
};

const openEditModal = (note: Note) => {
  modalMode.value = 'edit';
  currentNote.value = {
    id: note.id,
    title: note.title,
    content: note.content,
    createdAt: note.createdAt,
    updatedAt: note.updatedAt
  };
  showModal.value = true;
};

const openViewModal = (note: Note) => {
  modalMode.value = 'view';
  currentNote.value = {
    id: note.id,
    title: note.title,
    content: note.content,
    createdAt: note.createdAt,
    updatedAt: note.updatedAt
  };
  showModal.value = true;
};

const closeModal = () => {
  showModal.value = false;
  errorMessage.value = null;
  currentNote.value = {
    id: 0,
    title: '',
    content: '',
    createdAt: '',
    updatedAt: ''
  };
};

const handleSave = async () => {
  // Clear previous errors
  errorMessage.value = null;

  // Check for empty title
  if (!currentNote.value.title?.trim()) {
    errorMessage.value = 'Title is required and cannot be empty';
    return;
  }

  try {
    if (modalMode.value === 'create') {
      await notesStore.createNote({
        title: currentNote.value.title.trim(),
        content: currentNote.value.content?.trim() || ''
      });
      await notesStore.fetchNotes(); // Refresh to get updated pagination
    } else {
      await notesStore.updateNote(currentNote.value.id, {
        title: currentNote.value.title.trim(),
        content: currentNote.value.content?.trim() || ''
      });
      await notesStore.fetchNotes(); // Refresh
    }
    closeModal();
  } catch (error: any) {
    errorMessage.value =
      error.response?.data?.message || 'Failed to save note. Please try again.';
  }
};

const handleDelete = async (id: number) => {
  if (confirm('Are you sure you want to delete this note?')) {
    errorMessage.value = null;
    try {
      await notesStore.deleteNote(id);
    } catch (error: any) {
      errorMessage.value =
        error.response?.data?.message ||
        'Failed to delete note. Please try again.';
    }
  }
};

const handleSortChange = () => {
  notesStore.setSorting(sortBy.value, sortOrder.value);
};

const toggleSortOrder = () => {
  sortOrder.value = sortOrder.value === 'desc' ? 'asc' : 'desc';
  notesStore.setSorting(sortBy.value, sortOrder.value);
};

// Using formatTimestamp from dateFormat utils
</script>

<style scoped>
.line-clamp-3 {
  display: -webkit-box;
  -webkit-line-clamp: 3;
  -webkit-box-orient: vertical;
  overflow: hidden;
}
</style>
