<template>
    <header>
        <div class="interface">
        </div>
    </header>
    <main>
        <section class="topo-do-site"> <!-- Titulo e Barra de pesquisa-->
            <div class="interface">
                <div class="flex">

                    <h4 class="botao-novoFilme" @click="toggleNewMovieModal">Novo Filme <span
                            class="pi pi-plus-circle"></span></h4>
                    <h1 class="titulo">
                        Catálogo de Filmes
                    </h1>
                    <span class="pi pi-filter" @click="toggleFilterModal"></span>
                </div>
                <div class="search-bar">
                    <div class="input-container">
                        <span class="pi pi-search"></span>
                        <input type="text" v-model="searchQuery" placeholder="Buscar" />

                    </div>
                </div>
            </div>
        </section> <!--/ Titulo e barra de pesquisa -->

        <section> <!-- Novo Filme-->
            <div v-if="isNewMovieModalVisible" class="new-movie-modal">
                <div class="new-movie-modal-content">
                    <h2>Adicionar Novo Filme</h2>
                    <div class="form-group">
                        <label for="title">Título do Filme:</label>
                        <input type="text" id="title" v-model="newMovie.title" required>
                    </div>
                    <div class="form-group">
                        <label for="rating">Nota:</label>
                        <div class="rating-input">
                            <span v-for="star in 5" :key="star"
                                :class="['pi', star <= newMovie.rating ? 'pi-star-fill' : 'pi-star']"
                                @click="newMovie.rating = star"></span>
                        </div>
                    </div>
                    <div class="form-group">
                        <label for="genre">Gênero:</label>
                        <select id="genre" v-model="newMovie.genreId">
                            <option v-for="genre in genres" :key="genre.name" :value="genre.id">{{ genre.name }}
                            </option>
                        </select>
                    </div>
                    <div class="form-group">
                        <label for="releaseDate"> Data de Lançamento:</label>
                        <input type="date" id="releaseDate" v-model="newMovie.releaseDate" required>
                    </div>
                    <div class="form-group">
                        <label for="streaming">Streaming:</label>
                        <select id="streaming" v-model="newMovie.streamingId">
                            <option v-for="service in streaming" :key="service.name" :value="service.idStreaming">{{
                                service.name
                                }}</option>
                        </select>
                    </div>
                    <div class="form-group">
                        <label for="image">Imagem do Filme:</label>
                        <input type="file" id="image" @change="handleImageUpload" accept="image/*">
                    </div>
                    <div class="modal-actions">
                        <button @click="addNewMovie">Adicionar Filme</button>
                        <button @click="toggleNewMovieModal">Cancelar</button>
                    </div>
                </div>
            </div>
        </section> <!--/ Novo Filme -->

        <section> <!-- Filtros -->
            <div v-if="isFilterModalVisible" class="filter-modal">
                <div class="filter-modal-content">
                    <h2>Filtrar Filmes</h2>
                    <div class="filter-group">
                        <h3>Gênero</h3>
                        <select v-model="selectedGenreId">
                            <option value="">Todos os Generos</option>
                            <option v-for="genre in genres" :key="genre.id" :value="genre.id">{{ genre.name }}</option>
                        </select>
                    </div>

                    <div class="filter-group">
                        <h3>Streaming</h3>
                        <select v-model="selectedStreamingId">
                            <option value="">Todos os Streamings</option>
                            <option v-for="service in streaming" :key="service.idStreaming"
                                :value="service.idStreaming">{{
                                    service.name }}</option>
                        </select>
                    </div>

                    <div class="filter-group">
                        <h3>Avaliação</h3>
                        <input type="checkbox" v-model="highRatingOnly" /> Somente filmes com avaliação acima de 4.5
                        estrelas
                    </div>

                    <div class="filter-actions">
                        <button @click="applyFilters">Aplicar Filtros</button>
                        <button @click="clearFilters">Limpar Filtros</button>
                    </div>
                </div>
            </div>
        </section> <!--/ Filtros -->

        <section class="filmes"> <!-- Lista de Filmes -->
            <div class="interface">
                <div class="flex">
                    <div id="app" class="movie-list">
                        <div v-for="movie in filteredMovies" :key="movie.idMovie" class="movie-box"
                            @click="selectMovie(movie)">
                            <img :src="movie.imageBase64" alt="Movie Image" />
                            <div class="movie-info">
                                <h2>{{ movie.title }}</h2>
                                <div class="rating">
                                    <span v-for="star in 5" :key="star"
                                        :class="['pi', star <= movie.averageRating ? 'pi-star-fill' : 'pi-star']"></span>
                                </div>
                                <p>Gênero: {{ movie.genreName }}</p>
                                <p>Streaming: {{ movie.streamingName }}</p>
                                <p>Data de Lançamento: {{ movie.releaseDate }}</p>
                                <span class="pi pi-trash" style="margin-right: 30px;"
                                    @click.stop="openDeleteConfirmation(movie)"></span>
                            </div>
                        </div>
                        <MovieDetails v-if="selectedMovie" :movie="selectedMovie" @close="closeMovieDetails" />
                    </div>
                </div>
            </div>
            <!-- Modal Exclusao Filmes -->
            <div v-if="isDeleteConfirmationVisible" class="delete-confirmation-modal">
                <div class="delete-confirmation-content">
                    <h2>Você tem certeza que quer excluir esse filme?</h2>
                    <p>{{ movieToDelete?.title }}</p>
                    <div class="delete-actions">
                        <button @click="confirmDelete">Sim</button>
                        <button @click="cancelDelete">Não</button>
                    </div>
                </div>
            </div>
        </section>
    </main>
</template> <!-- Fim HTML -->


<script lang="ts">
import { defineComponent } from 'vue';
import api from './api/api';
import MovieDetails from './components/MovieDetails.vue';


export default defineComponent({
    data() {
        return {
            movieToDelete: null as any | null,
            movies: [] as any[],
            resposta: '',
            searchQuery: '',
            isFilterModalVisible: false,
            selectedMovie: null as any | null,
            selectedGenreId: '',
            selectedStreamingId: '',
            genres: [] as { id: number, name: string }[],
            streaming: [] as { idStreaming: number, name: string }[],
            highRatingOnly: false,
            isNewMovieModalVisible: false,
            isMovieDetailsVisible: false,
            isDeleteConfirmationVisible: false,
            newMovie: {
                title: '',
                rating: 0,
                genreId: null as number | null,
                streamingId: null as number | null,
                image: '', // Caso esteja utilizando upload de imagem
                releaseDate: '',
            },
        };
    },
    computed: {
        processedMovies() {
            return this.movies.map(movie => ({
                ...movie,
                genreName: this.getGenreName(movie.genreId),
                streamingName: this.getStreamingName(movie.streamingId),
                averageRating: this.getRating(movie.rating),
                releaseDate: new Date(movie.releaseDate).toLocaleDateString('pt-BR', { year: 'numeric', month: '2-digit', day: '2-digit'})
            }));
        },
        filteredMovies() {
            return this.processedMovies.filter(movie => {
                const genreMatch = !this.selectedGenreId || movie.genreId === this.selectedGenreId;
                const streamingMatch = !this.selectedStreamingId || movie.streamingId === this.selectedStreamingId;
                const ratingMatch = !this.highRatingOnly || movie.averageRating > 4.5;
                const searchMatch = !this.searchQuery || movie.title.toLowerCase().includes(this.searchQuery.toLowerCase());

                return genreMatch && streamingMatch && ratingMatch && searchMatch;
            });
        }
    },
    components: {
        MovieDetails
    },
    methods: {
        selectMovie(movie: any) {
            this.selectedMovie = movie;
        },
        closeMovieDetails() {
            this.selectedMovie = null;
        },
        toggleFilterModal() {
            this.isFilterModalVisible = !this.isFilterModalVisible;
        },
        applyFilters() {
            this.toggleFilterModal(); // Fecha o modal após aplicar os filtros
        },
        clearFilters() {
            this.selectedGenreId = '';
            this.selectedStreamingId = '';
            this.highRatingOnly = false;
            this.searchQuery = '';
        },
        toggleNewMovieModal() {
            this.isNewMovieModalVisible = !this.isNewMovieModalVisible;
        },
        handleImageUpload(event: Event) {
            const target = event.target as HTMLInputElement;
            const file = target.files?.[0];
            if (file) {
                this.newMovie.image = file;  // Aqui você armazena o arquivo
            }
        },
        openDeleteConfirmation(movie: null) {
            this.isDeleteConfirmationVisible = true;
            this.movieToDelete = movie;
        },
        async confirmDelete() {
            if (this.movieToDelete) {
                try {
                    await api.delete(`/Movies/${this.movieToDelete.idMovie}`);
                    this.movies = this.movies.filter(movie => movie.idMovie !== this.movieToDelete?.idMovie);
                    this.isDeleteConfirmationVisible = false;
                    this.movieToDelete = null;
                } catch (error) {
                    console.error('Erro ao excluir filme:', error);
                    this.resposta = 'Erro ao excluir filme';
                }
            }


        },
        cancelDelete() {
            this.isDeleteConfirmationVisible = false;
            this.movieToDelete = null;
        },
        async loadFilters() {
            try {
                const genresResponse = await api.get('/genres');
                this.genres = genresResponse.data;

                const servicesResponse = await api.get('/streamings');
                this.streaming = servicesResponse.data;
            } catch (error) {
                console.error('Erro ao carregar Filtros:', error);
            }

        },
        addNewMovie() {
            const formData = new FormData();
            formData.append('title', this.newMovie.title);
            formData.append('rating', this.newMovie.rating.toString());
            formData.append('genreId', this.newMovie.genreId?.toString() || '');
            formData.append('streamingId', this.newMovie.streamingId?.toString() || '');
            formData.append('releaseDate', this.newMovie.releaseDate);
            // Apenda o arquivo ao invés da URL
            if (this.newMovie.image instanceof File) {
                formData.append('image', this.newMovie.image);
            }

            api.post('/movies', formData, {
                headers: {
                    'Content-Type': 'multipart/form-data'
                }
            })
                .then(response => {
                    this.movies.push(response.data);
                    this.toggleNewMovieModal();
                    this.resetNewMovieForm();
                })
                .catch(error => {
                    console.error('Erro ao adicionar o filme:', error);
                    alert('Ocorreu um erro ao adicionar o filme.');
                });
        },
        resetNewMovieForm() {
            this.newMovie = {
                title: '',
                rating: 0,
                genreId: null,
                streamingId: null,
                image: '',
                releaseDate: '',
            };
        },
        async fetchData() {
            try {
                const [moviesResponse, genresResponse, streamingResponse] = await Promise.all([
                    api.get('/Movies'),
                    api.get('/Genres'),
                    api.get('/Streamings')
                ]);

                this.movies = moviesResponse.data;
                this.genres = genresResponse.data;
                this.streaming = streamingResponse.data;
            } catch (error) {
                console.error('Erro ao carregar dados:', error);
                this.resposta = 'Erro ao carregar dados';
            }
        },

        getGenreName(GenreId: number) {
            const genre = this.genres.find(g => g.id === GenreId);
            return genre ? genre.name : 'Desconhecido';
        },

        getStreamingName(streamingId: number) {
            const streaming = this.streaming.find(s => s.idStreaming === streamingId);
            return streaming ? streaming.name : 'Desconhecido';
        },
        getRating(Rating: number) {
            const rating = Math.round(Rating);
            return rating ? rating : 0;
        }
    },

    mounted() {
        this.loadFilters();
        // this.getMovies();
        this.fetchData();
    }
});
</script>

<style>
/* Gerais */
* {
    margin: 0;
    padding: 0;
    box-sizing: border-box;
    font-family: 'Poppins', sans-serif;
}

.interface {
    max-width: 1440px;
    margin: 0 auto;
}

body {
    background-color: rgb(34, 34, 34);
    height: 200vh;
    color: rgb(255, 255, 255);
    transition: background-color 0.3s, color 0.3s;
}

.flex {
    display: flex;
}

section.topo-do-site .interface,
section.filmes .interface {
    max-width: 1440px;
    margin: 0 auto;
}

section.topo-do-site {
    padding: 100px 5%;
}

section.topo-do-site .flex,
section.filmes .flex {
    display: flex;
    justify-content: center;
    width: 100%;
}

/* Header */
header {
    padding: 40px 4%;
    background-color: rgba(49, 49, 49, 0.466);
}

header>.interface {
    display: flex;
    justify-content: space-between;
    align-items: center;
}

/* Topo do Site */

section.topo-do-site .flex {
    align-items: center;
    position: relative;
    max-width: 1000px;
    margin: 0 auto;
    margin-bottom: 20px;
}

.titulo {
    text-align: center;
    color: rgb(255, 255, 255);
    /* margin-bottom: 30px; */
    position: relative;
    padding-right: 40px;
}

.search-bar {
    margin-bottom: 30px;
    border-radius: 20px;
    width: 100%;
    max-width: 1000px;
    margin-left: auto;
    margin-right: auto;
}

.input-container {
    position: relative;
    width: 100%;
    margin: 0 auto;
}

.input-container input {
    padding-left: 35px;
    width: 100%;
    padding-right: 10px 15px 10px 35px;
    border-radius: 20px;
    border: 1px solid rgb(255, 255, 255);
    height: 30px;
}

.input-container .pi-search {
    position: absolute;
    left: 10px;
    top: 50%;
    transform: translateY(-50%);
    color: #ebeaea;
    pointer-events: none;
}

.filter-options {
    display: flex;
    justify-content: space-between;
    margin-bottom: 20px;
}

.filter-options button {
    margin-right: 10px;
}

.botao-novoFilme {
    cursor: pointer;
    color: rgb(255, 255, 255);
    border: black;
    position: absolute;
    left: 0;
    top: 50%;
    transform: translateY(-50%);
}

.botao-novoFilme span {
    transform: translateY(15%);
}

.pi-filter {
    cursor: pointer;
    color: #ffffff;
    position: absolute;
    right: 0;
    top: 50%;
    transform: translateY(-50%);
}

/* Modal de Filtros */
.filter-modal {
    position: fixed;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    background-color: rgba(0, 0, 0, 0.466);
    display: flex;
    justify-content: center;
    align-items: center;
    z-index: 1000;
}


.filter-modal-content {
    background-color: rgb(43, 43, 43);
    padding: 20px;
    border-radius: 10px;
    width: 300px;
}

.filter-group {
    margin-bottom: 20px;
}

.filter-actions {
    display: flex;
    justify-content: space-between;
}

.filter-actions button {
    padding: 10px 20px;
    border: none;
    border-radius: 5px;
    background-color: #000000;
    color: white;
    cursor: pointer;
    transition: background-color 0.3s;
}

.filter-actions button:hover {
    background-color: rgb(95, 95, 95);
}

/* Filmes */

section.filmes .flex {
    gap: 60px;
}

.filmes .movie-box {
    color: rgb(255, 255, 255);
    display: flex;
    background-color: black;
    padding: 30px;
    border-radius: 20px;
    font-size: 14px;
    width: 380px;
    max-width: 500px;
    border: 1px solid #000000;
    transition: 0.8s ease-in-out;
    box-shadow: 0 2 16px rgba(255, 255, 255, 0.651);
    align-items: center;
}

.filmes .movie-box:hover {
    transform: scale(1.1);
    box-shadow: 0 0 6px rgba(190, 190, 190, 0.651);

}

.movie-collection {
    color: #ffffff;
    padding: 20px 5%;
    /* min-height: 100vh;  */
}


.movie-list {
    display: flex;
    flex-wrap: wrap;
    justify-content: center;
    gap: 45px;
}


.movie-box img {
    width: 100px;
    height: auto;
    margin-right: 20px;
    border-radius: 10px;
}

.movie-info {
    width: 280px;
    display: flex;
    flex-direction: column;
    padding: 20px;
    word-wrap: break-word;
}

.rating {
    margin-top: 40px;
}

.star {
    display: inline-flex;
    width: 20px;
    height: 20px;
    background-color: #ccc;
    margin-right: 5px;

}

/* Lixeira  */
.pi-trash {
    cursor: pointer;
    color: #ff0000;
    top: 50%;
    transform: translateX(130%) translateY(210%);
    z-index: 1;
}

.delete-confirmation-modal {
    position: fixed;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    background-color: rgba(0, 0, 0, 0.603);
    display: flex;
    justify-content: center;
    align-items: center;
    z-index: 1000;
}

.delete-confirmation-content {
    background-color: rgb(255, 255, 255);;
    padding: 30px;
    border-radius: 10px;
    width: 400px;
    max-width: 90%;
    color: black;
    text-align: center;
}

.delete-actions {
    display: flex;
    justify-content: space-around;
    margin-top: 20px;
}

.delete-actions button {
    padding: 10px 20px;
    border: none;
    border-radius: 5px;
    cursor: pointer;
    transition: background-color 0.3s;
}

.delete-actions button:first-child {
    background-color: #ff0000;
    color: white;
}

.delete-actions button:last-child {
    background-color: #000000;
    color: white;
}

.delete-actions button:hover {
    opacity: 0.8;
}


/* Modal Novo Filme */
.new-movie-modal {
    position: fixed;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    background-color: rgba(0, 0, 0, 0.603);
    display: flex;
    justify-content: center;
    align-items: center;
    z-index: 1000;
}

.new-movie-modal-content {
    background-color: rgb(43, 43, 43);
    padding: 30px;
    border-radius: 10px;
    width: 400px;
    max-width: 90%;
    color: rgb(226, 226, 226);
}

.form-group {
    margin-bottom: 20px;
}

.form-group label {
    display: block;
    margin-bottom: 5px;
}

.form-group input,
.form-group select {
    width: 100%;
    padding: 8px;
    border: 1px solid #ffffff;
    border-radius: 4px;
}

.rating-input {
    display: flex;
    gap: 5px;
}

.rating-input span {
    cursor: pointer;
    font-size: 24px;
}

.modal-actions {
    display: flex;
    justify-content: space-between;
    margin-top: 20px;
}

.modal-actions button {
    padding: 10px 20px;
    border: none;
    border-radius: 5px;
    background-color: #000000;
    color: white;
    cursor: pointer;
    transition: background-color 0.3s;
}

.modal-actions button:hover {
    background-color: rgb(95, 95, 95);
}
</style>