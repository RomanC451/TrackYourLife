import { toast } from "sonner";


type ExerciseDeletedToastProps = {
    name: string;
}

const exerciseDeletedToast = ({ name }: ExerciseDeletedToastProps) => {
    toast.success(`${name}`, {
        description: `Exercise deleted successfully`,
    });
}

export default exerciseDeletedToast;
    