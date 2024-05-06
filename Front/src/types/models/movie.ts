export type Movie = {
    movieID: number;
    title: string;
    releaseYear: number;
    directorID?: number;
    genreID?: number;
    posterPath: string;
    trailerPath: string;
};