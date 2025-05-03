import { Tile } from './tile.model';

export interface Board {
  id: string;
  width: number;
  height: number;
  tiles: Tile[];
}
