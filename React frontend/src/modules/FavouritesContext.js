import React, { useState, createContext } from 'react';

export const FavouritesContext = createContext();

export const FavouritesProvider = (props) => {
    const saved = localStorage.getItem("favouritePokemon");

    // Favourites schema example: [{pokedex_number: 23, dateTime: "04/15/2023, 10:22:29 PM"},...]
    const [favourites, setFavourites] = useState(JSON.parse(saved) || []);

    function addFavourite(newFavouritePokemon) {
        const isDuplicate = favourites.some(item => item.pokedex_number === newFavouritePokemon.pokedex_number);

        if (!isDuplicate) {
            const newFavourites = [...favourites, newFavouritePokemon];
            setFavourites(newFavourites);
            localStorage.setItem("favouritePokemon", JSON.stringify(newFavourites));
        }
    }

    function removeFavourite(pokedexNum) {
        const newFavourites = favourites.filter(item => item.pokedex_number !== pokedexNum);
        setFavourites(newFavourites);
        // Update localStorage with the new array
        localStorage.setItem("favouritePokemon", JSON.stringify(newFavourites));
    }

    return (
        <FavouritesContext.Provider value={{ favourites, addFavourite, removeFavourite }}>
            {props.children}
        </FavouritesContext.Provider>
    );
}