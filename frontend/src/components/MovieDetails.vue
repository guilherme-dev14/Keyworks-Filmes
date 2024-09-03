<template>
  <div class="movie-details-modal" v-if="movie">
    <div class="movie-details-content">
      <div class="movie-info">
        <div class="movie-image">
          <img :src="movie.imageBase64" alt="Movie Cover" class="movie-cover" />
        </div>
        <div class="movie-meta">
          <h1>{{ movie.title }}</h1>
          <div class="rating">
            <span v-for="star in 5" :key="star"
              :class="['pi', star <= movie.averageRating ? 'pi-star-fill' : 'pi-star']"></span>
            <span>({{ movie.averageRating }})</span>
          </div>
          <p>Gênero: {{ movie.genreName }}</p>
          <p>Streaming: {{ movie.streamingName }}</p>

          <p>Data de Lançamento: {{ movie.releaseDate }}</p>
        </div>
      </div>
      <div class="comments-section">
  
        
        <div class="new-comment">
          <textarea v-model="newComment" placeholder="Adicionar um comentário"></textarea>
          <button @click="addComment">Adicionar Comentário</button>
        </div>
        <h2>Comentários</h2>
        <ul>
          <li v-for="comment in comments" :key="comment.idMovie"> {{ comment.comment }}</li>
        </ul>
      </div>

      <button class="close-button" @click="$emit('close')">Fechar</button>
    </div>
  </div>
</template>

<script>
import api from '../api/api';

export default {
  props: {
    movie: Object,
  },
  data() {
    return {
      comments: [],
      newComment: '',
    };
  },
  methods: {
    async fetchComments() {
      try {
        const response = await api.get(`/Reviews/${this.movie.idMovie}`);
        this.comments = response.data;
      } catch (error) {
        console.error('Erro ao buscar Filmes:', error);
      }
    },
    async addComment() {
      if (!this.newComment.trim()) {
        alert('Por favor, digite um comentário antes de adicionar.');
        return;
      }

      try {
        const payload = {
          movieId: this.movie.idMovie, // Alterado de idMovie para movieId
          comment: this.newComment,
          // Adicione outros campos necessários para o Review, se houver
        };

        console.log('Payload sendo enviado:', payload);

        const response = await api.post('/Reviews', payload);

        console.log('Resposta recebida:', response);

        if (response.status === 201 || response.status === 200) {
          this.comments.push(response.data);
          this.resetCommentForm();
        } else {
          throw new Error(`Resposta inesperada: ${response.status}`);
        }
      } catch (error) {
        console.error('Erro detalhado ao adicionar o comentário:', error);
        
        let errorMessage = 'Ocorreu um erro ao adicionar o comentário. ';
        
        if (error.response) {
          console.error('Dados da resposta de erro:', error.response.data);
          console.error('Status do erro:', error.response.status);
          errorMessage += `Código de status: ${error.response.status}. `;
          errorMessage += `Mensagem: ${error.response.data}`;
        } else if (error.request) {
          console.error('Erro na requisição:', error.request);
          errorMessage += 'Não foi possível conectar ao servidor.';
        } else {
          console.error('Erro de configuração:', error.message);
          errorMessage += `Erro: ${error.message}`;
        }
        
        alert(errorMessage);
      }
    },
    resetCommentForm() {
      this.newComment = '';
    },
  },
  mounted() {
    this.fetchComments();
  },
};
</script>

<style scoped>
.movie-details-modal {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background-color: rgba(0, 0, 0, 0.397);
  display: flex;
  justify-content: center;
  align-items: center;
  z-index: 1000;
}

.movie-details-content {
  background-color: rgb(0, 0, 0);
  padding: 30px;
  border-radius: 10px;
  width: 80%;
  max-width: 1000px;
  max-height: 90%;
  overflow-y: auto;
  color: rgb(255, 255, 255);

}

.movie-info {
  display: flex;
  flex-direction: row;
  gap: 20px;
  margin-bottom: 30px;
  width: fit-content;
}

.movie-image {
  flex: 1;
  margin-right: 30px;

}

.movie-cover {
  width: 200px;
  height: auto;
  border-radius: 10px;
  object-fit: cover;
}

.movie-meta {
  flex: 2;
  display: flex;
  flex-direction: column;
  justify-content: flex-start;
}

.movie-meta h1 {
  margin-top: 0;
  margin-bottom: 15px;
}

.rating {
  margin-bottom: 15px;
}

.rating .pi {
  color: gold;
  font-size: 1.2em;
}

.movie-meta p {
  margin: 10px 0;
}

.comments-section {
  margin-top: 30px;
  border-top: 1px solid #ccc;
  padding-top: 20px;
  clear: both;
}

.comments-section h2 {
  margin-bottom: 15px;
}

.comments-section ul {
  list-style-type: none;
  padding: 0;
}

.comments-section li {
  background-color: #242424;
  font-size: 20px;
  padding: 10px;
  margin-bottom: 5px;
  border-radius: 5px;
}

.new-comment {
  margin-top: 10px;
}

.new-comment textarea {
  width: 100%;
  height: 80px;
  margin-bottom: 3px;
  padding: 10px;
  border-radius: 5px;
  border: 1px solid #ccc;
}

.new-comment button,
.close-button {
  padding: 10px 20px;
  background-color: #383838;
  color: #fff;
  border: none;
  border-radius: 5px;
  cursor: pointer;
  transition: background-color 0.3s;
  margin-bottom: 20px;
}

.new-comment button:hover,
.close-button:hover {
  background-color: #333;
}

.close-button {
  margin-top: 20px;
  display: block;
  margin-left: auto;
}
</style>