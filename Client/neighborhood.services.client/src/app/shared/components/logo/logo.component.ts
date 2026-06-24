import { Component } from '@angular/core';

/**
 * Animated brand mark for Neighborhood Services: a home at the centre with a radar
 * sweep fanning out around it — "services found near you". Blue/white, slow + calm.
 * Drop it in anywhere with <app-logo />.
 *
 * To stop the motion later, remove the `ns-sweep` / `ns-blip` animation lines — the
 * artwork still renders correctly without them.
 */
@Component({
  selector: 'app-logo',
  standalone: true,
  template: `
    <svg class="ns-logo" viewBox="0 0 48 48" role="img" aria-label="Neighborhood Services" xmlns="http://www.w3.org/2000/svg">
      <defs>
        <linearGradient id="nsRadarGrad" x1="0" y1="0" x2="1" y2="1">
          <stop offset="0" stop-color="#60a5fa" />
          <stop offset="1" stop-color="#1d4ed8" />
        </linearGradient>
        <clipPath id="nsRadarClip"><rect x="2" y="2" width="44" height="44" rx="14" /></clipPath>
      </defs>

      <rect x="2" y="2" width="44" height="44" rx="14" fill="url(#nsRadarGrad)" />

      <g clip-path="url(#nsRadarClip)">
        <!-- radar grid -->
        <circle cx="24" cy="24" r="16.5" fill="none" stroke="#fff" stroke-opacity=".22" stroke-width="1" />
        <circle cx="24" cy="24" r="11" fill="none" stroke="#fff" stroke-opacity=".22" stroke-width="1" />
        <circle cx="24" cy="24" r="5.5" fill="none" stroke="#fff" stroke-opacity=".22" stroke-width="1" />

        <!-- the sweep: a faint wide trail + a bright leading beam, turning slowly -->
        <g class="ns-sweep">
          <path d="M24,24 L24,7.5 A16.5 16.5 0 0 1 34.6,36.9 Z" fill="#fff" opacity=".10" />
          <path d="M24,24 L24,7.5 A16.5 16.5 0 0 1 39.5,18.6 Z" fill="#fff" opacity=".30" />
          <line x1="24" y1="24" x2="24" y2="7.5" stroke="#fff" stroke-width="1.4" stroke-opacity=".9" />
        </g>

        <!-- a "found service" blip pinging on the grid -->
        <circle class="ns-blip" cx="33.5" cy="16.5" r="1.7" fill="#fff" />

        <!-- home at the centre -->
        <polygon points="24,19 28.6,23 19.4,23" fill="#fff" />
        <rect x="20" y="22.5" width="8" height="6.2" rx="0.9" fill="#fff" />
        <rect x="22.6" y="25.4" width="2.8" height="3.3" rx="0.4" fill="#1d4ed8" />
      </g>
    </svg>
  `,
  styles: [`
    .ns-logo {
      width: 2.2rem;
      height: 2.2rem;
      display: inline-block;
      flex-shrink: 0;
      filter: drop-shadow(0 2px 6px rgba(29, 78, 216, 0.28));
    }

    /* Slow, calm radar rotation around the badge centre (view-box coords). */
    .ns-sweep {
      transform-box: view-box;
      transform-origin: 24px 24px;
      animation: nsSweep 7s linear infinite;
    }
    @keyframes nsSweep { to { transform: rotate(360deg); } }

    .ns-blip { animation: nsBlip 2.6s ease-in-out infinite; }
    @keyframes nsBlip {
      0%, 100% { opacity: 0; }
      40%      { opacity: 1; }
      70%      { opacity: 0.2; }
    }

    @media (prefers-reduced-motion: reduce) {
      .ns-sweep, .ns-blip { animation: none; }
      .ns-blip { opacity: 0.9; }
    }
  `],
})
export class LogoComponent {}
