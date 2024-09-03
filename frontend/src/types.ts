export interface Genre {
    id: number;
    name: string;
  }
  
  export interface streaming{
    idStreaming: number;
    name: string;
  }
  
  export interface Movie {
    id: number;
    title: string;
    rating: number;
    genre: string;
    streaming: string;
    image: string;
  }