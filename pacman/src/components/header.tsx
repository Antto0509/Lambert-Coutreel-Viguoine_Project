import '../styles/header.css';

export default function Header() {
  return (
    <header>
        <img src="pacman.png" alt="Pacman" /><span>Pacman</span>
        <nav className="menu">
            <a href="/">Home</a>
            <a href="/play">Play</a>
            <a href="/scores">Scores</a>
            <a href="/about">About</a>
        </nav>
    </header>
    )
}