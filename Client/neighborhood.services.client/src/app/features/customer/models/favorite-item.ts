import { FavoriteTechnician } from "./favorite-technician";

export class FavoriteItem {

    favoriteId!: number;
    technicianId!: number;
    technicianName: string= '';
    imageURL: string = '';
    customerId!: number;
    addedDate!: Date;
    technician!: FavoriteTechnician;
}
