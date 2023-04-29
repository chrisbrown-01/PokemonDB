import React from 'react';
import SearchBar from './SearchBar';
import { useNavigate } from 'react-router-dom';

function Home() {
    const navigate = useNavigate();

    const searchBarItemSelected = (pokemon) => {
        navigate(`/pokemon-stats/${pokemon.name}`)
    }

    return (
        <>
            <div>
                <div className="center">
                    <h1>Welcome to PokemonDB!</h1>
                    <SearchBar onListItemClick={(pokemon) => searchBarItemSelected(pokemon)} />
                </div>
                <p>Search for any Pokemon using the Search bar.</p>
                <p>View all Pokemon profiles <a href="/profiles">here</a>. Click any image to view stats.</p>
                <p>View your favourites list <a href="/favourites">here</a>.</p>
                <p>Make advanced searches for Pokemon based on stats and keywords <a href="/advancedsearch"> here</a>.</p>
                <p>View backend Swagger API endpoints <a href="http://localhost:55558/swagger" target="_blank" rel="noopener noreferrer">here</a>.</p>
                <p>View services health check results <a href="http://localhost:55558/health" target="_blank" rel="noopener noreferrer">here</a>.</p>
                <p>View Watchdog logs <a href="http://localhost:55558/watchdog" target="_blank" rel="noopener noreferrer">here</a>. (<i>Username:</i> <b>SA</b> - <i>Password:</i> <b>StrongPassword@1</b>)</p>
                <div className="center">
                    <img src="/pikachu.gif" alt="Pikachu Rotating Gif"></img>
                </div>
            </div>
        </>
    );
}
export default Home;