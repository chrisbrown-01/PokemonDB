import React, { useState, useEffect, useRef } from 'react';

function SearchBar({ onListItemClick }) {
    const [searchInput, setSearchInput] = useState("");
    const [searchResults, setSearchResults] = useState([]);
    const [pokemonResponse, setPokemonResponse] = useState([]);

    const inputRef = useRef();

    useEffect(() => {
        const fetchData = async () => {
            try {
                const response = await fetch('http://localhost:55558/api/pokemon/namesNumsImages');
                if (response.status === 200) {
                    const data = await response.json();
                    setPokemonResponse(data);
                } else {
                    console.log(`Request failed with status code ${response.status}`);
                    setPokemonResponse([]);
                }
            } catch (error) {
                console.log(error.message);
                setPokemonResponse([]);
            }
        };
        fetchData();
        inputRef.current.focus();
    }, []);

    const searchForPokemon = (e) => {
        setSearchInput(e.target.value);
        if (e.target.value === "") {
            setSearchResults([]);
        } else {
            const results = pokemonResponse.filter(item => item.name.toLowerCase().startsWith(e.target.value.toLowerCase()));
            setSearchResults(results);
        }
    }

    return (
        <div>
            <div className="searchbar">
                <input
                    type="search"
                    value={searchInput}
                    onChange={e => searchForPokemon(e)}
                    placeholder="Search Pokemon..."
                    ref={inputRef}
                />
                {searchResults.length > 0 && (
                    <ul>
                        {searchResults.slice(0, 5).map((post, key) =>
                            <li key={key} onClick={() => onListItemClick(post)}>
                                {post.name}
                            </li>
                        )}
                    </ul>
                )}
            </div>
        </div>
    )
}
export default SearchBar;