import React, { useState, useEffect } from 'react';

function SidePanel() {
    const [pokemonList, setPokemonList] = useState([]);

    useEffect(() => {
        const fetchPokemon = async () => {
            try {
                const response = await fetch('http://localhost:55558/api/pokemon/namesNumsImages');

                if (response.status === 200) {
                    const data = await response.json();
                    setPokemonList(data);
                }
                else {
                    console.log(`Request failed with status code ${response.status}`);
                    setPokemonList([]);
                }
            }
            catch (error) {
                console.log(error.message);
            }
        };
        fetchPokemon();
    }, []);

    const pokemonListRender = pokemonList.map((item, index) => (
        <div key={index}>
            <a href={`/pokemon-stats/${item.name}`}>
                {item.pokedex_number}. {item.name}
            </a>
        </div>
    ))

    return (
        <div>
            <h3 className="center">Quick Access List</h3>
            {pokemonListRender}
        </div>
    );
}

export default SidePanel;